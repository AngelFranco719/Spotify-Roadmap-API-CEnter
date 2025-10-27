using SpotifyRequestManagement.Models.Multiple_Entities;
using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Models.Simplified_Entities;
using SpotifyRequestManagement.Models.Pages;
using System.Collections.Concurrent;
using System.Threading.Tasks;

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
        int artistas = 0;

        // Constructor
        public MusicalGenerativeField(LastFMApiRequest _lastFMApi, 
            ILogger<MusicalGenerativeField> _logger, 
            SpotifyApiRequest _spotifyApiRequest, 
            TreeFlattener contextualFilter
            ) {
            this.lastFMApi = _lastFMApi;
            this.logger = _logger;
            this.spotifyApiRequest = _spotifyApiRequest;
        }

        // Hilo principal
        public async Task<ConcurrentBag<SimplifiedTrack>> mainThread()
        {
            this.queueRequestArtists.Enqueue(await getMainArtist());
            await GenerativeProcess();
            return sample;
        }

        // Obtener al artista correcto 
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

        // Generador del árbol mediante BFS
        private async Task GenerativeProcess() {
            ConcurrentQueue<Artist> newLevel = new ConcurrentQueue<Artist>();
            List<Task> tasks = new List<Task>(); 
            while (this.queueRequestArtists.Count != 0 && artistas <= 20) {
                queueRequestArtists.TryDequeue(out Artist current);
                tasks.Add(getRelatedArtists(current, newLevel));
                artistas++; 
            }
            await Task.WhenAll(tasks);
            queueRequestArtists = newLevel; 
            if (artistas <= 20)
                await GenerativeProcess(); 
        }

        #region Obtener los artistas relacionados
        private async Task getRelatedArtists(Artist current_artist, ConcurrentQueue<Artist> newLevel) {
            SimilarArtists similarArtists = await lastFMApi.getSimilarArtists(current_artist.name);
            List<FMArtist> expanded = extractRelevantArtist(similarArtists);
            await MapFMToSpotify(expanded, current_artist, newLevel); 
        }
        #endregion

        #region Extraer Artistas Más Relevantes

        private List<FMArtist> extractRelevantArtist(SimilarArtists current) {
            int priority = 8;
            int randomNumber = 2;

            if (current.artist.Count == 0)
                return new List<FMArtist>(); 
            List<FMArtist> selectedArtists = new List<FMArtist> { current.artist[0] };
            List<FMArtist> remaining = current.artist.Skip(1).Take(priority).ToList();
            selectedArtists.AddRange(remaining.OrderBy(x => random.Next()).Take(randomNumber));
            return selectedArtists;
        }

        #endregion

        #region Mapear Datos de LastFM a Spotify
        private async Task MapFMToSpotify(List<FMArtist> expandedArtists, Artist current_artist, ConcurrentQueue<Artist>newLevel) {
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
    }
}
