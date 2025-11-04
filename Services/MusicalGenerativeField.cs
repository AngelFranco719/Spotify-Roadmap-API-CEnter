using SpotifyRequestManagement.Models.Multiple_Entities;
using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Models.Simplified_Entities;
using SpotifyRequestManagement.Models.Pages;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SpotifyRequestManagement.Services
{
    public class MusicalGenerativeField
    {
        public Track? root { get; set; }
        private readonly LastFMApiRequest lastFMApi;
        private readonly SpotifyApiRequest spotifyApiRequest;
        ConcurrentQueue<Artist> queueRequestArtists = new ConcurrentQueue<Artist>();
        ConcurrentBag<SimplifiedTrack> sample = new ConcurrentBag<SimplifiedTrack>();
        private ConcurrentDictionary<string, bool> visitedArtists = new ConcurrentDictionary<string, bool>();


        private readonly SemaphoreSlim _artistLock = new SemaphoreSlim(1, 1);

        int deep = 0;
        private static Random random = new Random();
        private Artist? mainArtist;
        int artistas = 0;
        ILogger<MusicalGenerativeField> logger;
        IHubContext<ProgressHub> hub;

        // Constructor
        public MusicalGenerativeField(LastFMApiRequest _lastFMApi,
            SpotifyApiRequest _spotifyApiRequest,
            ILoggerFactory factory,
            IHubContext<ProgressHub> hub
            )
        {
            this.lastFMApi = _lastFMApi;
            this.spotifyApiRequest = _spotifyApiRequest;
            this.logger = factory.CreateLogger<MusicalGenerativeField>();
            this.hub = hub;
        }

        // Hilo principal
        public async Task<ConcurrentBag<SimplifiedTrack>> mainThread()
        {
            this.queueRequestArtists.Enqueue(await getMainArtist());
            await GenerativeProcess();
            return sample;
        }

        // Obtener al artista correcto 
        public async Task<Artist> getMainArtist()
        {
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
        private async Task GenerativeProcess()
        {
            ConcurrentQueue<Artist> newLevel = new ConcurrentQueue<Artist>();

            while (artistas < 20 && queueRequestArtists.Count > 0)
            {
                queueRequestArtists.TryDequeue(out Artist current); 
                await getRelatedArtists(current, newLevel);   
            }

            queueRequestArtists = newLevel;

            if (artistas < 20 && queueRequestArtists.Count > 0)
                await GenerativeProcess();
        }


        #region Obtener los artistas relacionados
        private async Task getRelatedArtists(Artist current_artist, ConcurrentQueue<Artist> newLevel)
        {
            SimilarArtists similarArtists = await lastFMApi.getSimilarArtists(current_artist.name);
            List<FMArtist> expanded = extractRelevantArtist(similarArtists);
            await MapFMToSpotify(expanded, current_artist, newLevel);
        }
        #endregion

        #region Extraer Artistas Más Relevantes

        private List<FMArtist> extractRelevantArtist(SimilarArtists current)
        {
            int priority = 8;
            int randomNumber = 5;

            if (current == null || current.artist == null || current.artist.Count == 0)
                return new List<FMArtist>();
            List<FMArtist> selectedArtists = new List<FMArtist> { current.artist[0] };
            List<FMArtist> remaining = current.artist.Skip(1).Take(priority).ToList();
            selectedArtists.AddRange(remaining.OrderBy(x => random.Next()).Take(randomNumber));
            return selectedArtists;
        }

        #endregion

        #region Mapear Datos de LastFM a Spotify
        private async Task MapFMToSpotify(List<FMArtist> expandedArtists, Artist current_artist, ConcurrentQueue<Artist> newLevel)
        {
            QuerySearch querySearch = new QuerySearch();
            querySearch.type = ["artist"];

            foreach (FMArtist artist in expandedArtists)
            {
                bool shouldProcess = false;
                await _artistLock.WaitAsync();
                try
                {
                    if (!visitedArtists.ContainsKey(artist.Name))
                    {
                        visitedArtists.TryAdd(artist.Name, true);
                        shouldProcess = true; 
                    }
                }
                finally
                {
                    _artistLock.Release();
                }
                if (!shouldProcess)
                {
                    logger.LogInformation("Artista {name} ya fue procesado", artist.Name);
                    continue;
                }
                querySearch.search = artist.Name;
                logger.LogInformation("Procesando artista: {key}", artist.Name);

                QueryResponse queryResponse = new QueryResponse(querySearch, spotifyApiRequest);
                var result = await queryResponse.getQueryResponse<Artist, Artists>();
                var parsedArtist = result.GetResult();
                Artist correctResult = parsedArtist.FirstOrDefault(c => string.Equals(c.name, artist.Name, StringComparison.OrdinalIgnoreCase));

                if (correctResult != null)
                {
                    current_artist.relatedArtist.Add(correctResult);
                    newLevel.Enqueue(correctResult);
                    artistas++;
                }
                else
                {
                    visitedArtists.TryRemove(artist.Name, out _);
                }
            }
        }
        #endregion
    }
}