using SpotifyRequestManagement.Models.Multiple_Entities;
using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Models.Simplified_Entities;
using SpotifyRequestManagement.Models.Pages;
using System.Collections.Concurrent;

namespace SpotifyRequestManagement.Services
{
    public class MusicalGenerativeField
    {
        public Track? root { get; set; }
        private readonly LastFMApiRequest lastFMApi;
        private readonly ILogger<MusicalGenerativeField> logger;
        private readonly SpotifyApiRequest spotifyApiRequest;
        ConcurrentQueue<Artist> queueRequestArtists = new ConcurrentQueue<Artist>();
        ConcurrentBag<SimplifiedTrack> sample = new ConcurrentBag<SimplifiedTrack>();
        private ConcurrentDictionary<string, bool> visitedArtists = new ConcurrentDictionary<string, bool>();
        int deep = 0;
        private static Random random = new Random();
        private Artist? mainArtist; 

        SamplerContextualFilter contextualFilter;  

        public MusicalGenerativeField(LastFMApiRequest _lastFMApi, 
            ILogger<MusicalGenerativeField> _logger, 
            SpotifyApiRequest _spotifyApiRequest, 
            SamplerContextualFilter contextualFilter
            ) {
            this.lastFMApi = _lastFMApi;
            this.logger = _logger;
            this.spotifyApiRequest = _spotifyApiRequest;
            this.contextualFilter = contextualFilter; 
        }

        public async Task<Artist> getMainArtist() {
            QuerySearch querySearch = new QuerySearch();
            querySearch.type = ["artist"];
            querySearch.search = root.artists[0].name;
            QueryResponse queryResponse = new QueryResponse(querySearch, spotifyApiRequest);
            var result = await queryResponse.getQueryResponse<Artist, Artists>();
            var parsedArtist = result.GetResult();
            Artist correctRoot = new Artist();
            foreach (Artist correct in parsedArtist)
            {
                if (correct.name == root.artists[0].name)
                {
                    correctRoot = correct;
                    break;
                }
            }
            this.mainArtist = correctRoot;
            queueRequestArtists.Enqueue(mainArtist);
            visitedArtists.TryAdd(mainArtist.name, true);
            return correctRoot; 
        }

        public async Task<ConcurrentBag<SimplifiedTrack>> mainThread() {
            await GenerativeProcess();
            return sample;
        }

        private async Task GenerativeProcess() {
            if (sample.Count > 500)
                return; 

            ConcurrentQueue<Artist> newLevel = new ConcurrentQueue<Artist>();
            List<Task> tasks = new List<Task>();
            while (queueRequestArtists.TryDequeue(out Artist current) && sample.Count <= 500 && deep < 10)
            {
                logger.LogInformation("\n\n----------------------------\n Proceso al artista {artist}\n", current.name);
                logger.LogInformation("\nLlevo {tam} canciones analizadas", sample.Count);
                tasks.Add(GetSimplifiedAlbumsFromArtist(current.id));
                tasks.Add(getRelatedArtists(current, newLevel));
            }
            await Task.WhenAll(tasks); 
            queueRequestArtists = newLevel;
            deep++;

            if (sample.Count <= 500)
                await GenerativeProcess();
        }

        #region Obtener los artistas relacionados
        private async Task getRelatedArtists(Artist current_artist, ConcurrentQueue<Artist> newLevel) {
            SimilarArtists similarArtists = await lastFMApi.getSimilarArtists(current_artist.name);
            List<FMArtist> expanded = extractRelevantArtist(similarArtists);
            await MapFMToSpotify(expanded, newLevel, current_artist); 
        }
        #endregion

        #region Extraer Artistas Más Relevantes
        private List<FMArtist> extractRelevantArtist(SimilarArtists current) {
            int priority = 8;
            int randomNumber = 2;
            List<FMArtist> selectedArtists = new List<FMArtist> { current.artist[0] };
            List<FMArtist> remaining = current.artist.Skip(1).Take(priority).ToList();

            selectedArtists.AddRange(remaining.OrderBy(x => random.Next()).Take(randomNumber));

            return selectedArtists; 
        }
        #endregion

        #region Mapear Datos de LastFM a Spotify
        private async Task MapFMToSpotify(List<FMArtist> expandedArtists, ConcurrentQueue<Artist> newLevel, Artist current_artist) {
            QuerySearch querySearch = new QuerySearch();
            querySearch.type = ["artist"];
            foreach (FMArtist artist in expandedArtists){
                querySearch.search = artist.Name;
                logger.LogInformation("Clave: {key}", artist.Name); 
                if (visitedArtists.TryAdd(artist.Name, true))
                {
                    QueryResponse queryResponse = new QueryResponse(querySearch, spotifyApiRequest);
                    var result = await queryResponse.getQueryResponse<Artist, Artists>();
                    var parsedArtist = result.GetResult();
                    Artist correctResult = parsedArtist.FirstOrDefault(c => string.Equals(c.name, artist.Name, StringComparison.OrdinalIgnoreCase));
                    if (correctResult != null)
                    {
                        current_artist.relatedArtist.Add(correctResult);
                        newLevel.Enqueue(correctResult);
                    }
                }
            }            
        }
        #endregion

        #region Obtener Albumes de un Artista
        private async Task GetSimplifiedAlbumsFromArtist(string artistID) {
            AlbumPages albumPages = await spotifyApiRequest.getArtistAlbums(artistID);
            await GetSpecifiedAlbumsFromArtist(albumPages); 
        }

        private async Task GetSpecifiedAlbumsFromArtist(AlbumPages albumPages) {
            foreach (SimplifiedAlbum simplifiedAlbum in albumPages.items) {
                Album current = await spotifyApiRequest.getAlbum(simplifiedAlbum.id);
                logger.LogInformation("{album}", current.ToString());
                mapSimplifiedTracksInSample(current); 
            }
        }

        private void mapSimplifiedTracksInSample(Album current) {
            Tracks tracksPagination = current.tracks;
            SimplifiedTrack[] simplifiedTracks = tracksPagination.items;
            foreach (SimplifiedTrack track in simplifiedTracks) {
                sample.Add(track);
            }
        }
        #endregion
    }
}
