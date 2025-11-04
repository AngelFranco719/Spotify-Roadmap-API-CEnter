using SpotifyRequestManagement.Models.Entities;
using System.Collections.Concurrent;
using SpotifyRequestManagement.Models.Simplified_Entities;
using SpotifyRequestManagement.Models;
using Microsoft.AspNetCore.SignalR;

namespace SpotifyRequestManagement.Services
{
    public class FilterAlgorithmHub
    {
        public Track? root; 
        public Artist? mainArtist { get; set; }
        public ConcurrentBag<SimplifiedTrack>? caothicSample;
        public List<Artist> graph = new List<Artist>();
        public Dictionary<string, Artist> refToArtist = new Dictionary<string, Artist>(); 
        public ConcurrentBag<Track> finalCaothicSample;      
        public List<Track> finalQueue;
        private readonly SpotifyApiRequest requestController;
        private readonly ILogger logger;
        private readonly LastFMApiRequest lastFMApiRequest;
        public MusicalGenerativeField generativeField;
        public MapSimplifiedTracks mapSimplifiedTracks;
        public TreeFlattener samplerContextualFilter;
        public TabuSearch tabu;
        public GeneratePlayQueue generatePlayQueue;
        public DeepFilteringSample deepFilteringSample;
        public GetTracksFromArtists getTracksFromArtists;
        public int duration { get; set; }
        private readonly IHubContext<ProgressHub> _hubContext;
        private readonly string connectionId;

        public FilterAlgorithmHub(SpotifyApiRequest requestController, ILoggerFactory loggerFactory, 
            LastFMApiRequest lastFMApiRequest, IHubContext<ProgressHub> _hubContext, string connectionID)
        { 
            logger = loggerFactory.CreateLogger<FilterAlgorithmHub>();
            this.requestController = requestController;
            this.lastFMApiRequest = lastFMApiRequest;
            this.mapSimplifiedTracks = new MapSimplifiedTracks(requestController, loggerFactory); 
            this.samplerContextualFilter = new TreeFlattener(loggerFactory, mapSimplifiedTracks);
            this.generatePlayQueue = new GeneratePlayQueue(loggerFactory);
            this.tabu = new TabuSearch(loggerFactory, generatePlayQueue); 
            this.deepFilteringSample = new DeepFilteringSample(tabu, loggerFactory);
            this._hubContext = _hubContext;
            this.generativeField = new MusicalGenerativeField(lastFMApiRequest, requestController, loggerFactory, _hubContext);
            this.connectionId = connectionID;
            this.getTracksFromArtists = new GetTracksFromArtists(requestController, loggerFactory, _hubContext, connectionID);
        }

        public async Task<List<Track>> MainThread(string ID_Root)
        {
            root = await requestController.getTrack(ID_Root);

            logger.LogInformation("Inicio el proceso"); 
            await this.getGenerativeField();

            logger.LogInformation("Busco canciones");

            await this.setContextualFilter(); 

            this.getTracksFromArtists.flattenedGraph = this.graph;
            caothicSample = await this.getTracksFromArtists.getTracks();
            await this.setMapSimplifiedTracks();
            mapTracksToDictionary();
            logger.LogInformation("Busco con tabu"); 
            this.tabu.refToArtist = this.refToArtist; 
            this.deepFilteringSample.root = this.root;
            this.deepFilteringSample.graph = this.graph;
            this.deepFilteringSample.tabuSearch = this.tabu;
            this.deepFilteringSample.mainThread();

            this.generatePlayQueue.filteredTracks = this.tabu.artistsTracks;

            string rootArtistId = root.artists[0].id;
            if (!this.generatePlayQueue.filteredTracks.ContainsKey(rootArtistId))
            {
                logger.LogWarning("El artista raíz ({id}) no está en filteredTracks. Se añadirá manualmente.", rootArtistId);
                this.generatePlayQueue.filteredTracks[rootArtistId] = new List<Track> { root };
            }

            foreach (string key in  this.generatePlayQueue.filteredTracks.Keys) {
                List<Track> current = this.generatePlayQueue.filteredTracks[key];
            }

            logger.LogInformation("Genero la Playlist final");
            this.finalQueue = this.generatePlayQueue.generatePlayQueue(root, duration);
            return finalQueue; 
        }

        public async Task getGenerativeField() {
            this.generativeField.root = this.root;
            this.mainArtist = await this.generativeField.getMainArtist();
            this.caothicSample = await this.generativeField.mainThread(); 
        }

        public async Task setContextualFilter() {
            await samplerContextualFilter.setRoot(mainArtist);
            this.graph = this.samplerContextualFilter.getBFS();
            await this._hubContext.Clients.Client(connectionId).SendAsync("getFeedback", graph); 
            graph.ForEach(x => refToArtist[x.id] = x); 
        }

        public async Task setMapSimplifiedTracks()
        {
            mapSimplifiedTracks.initialSample = this.caothicSample;
            this.finalCaothicSample = await mapSimplifiedTracks.setGroups();
        }

        public void mapTracksToDictionary()
        {
            foreach (Track item in finalCaothicSample)
            {
                string artist_id = item.artists[0].id;
                if (!this.tabu.artistsTracks.TryGetValue(artist_id, out var listTrack))
                {
                    listTrack = new List<Track>();
                    this.tabu.artistsTracks[artist_id] = listTrack;
                }
                listTrack.Add(item);
            }

            foreach (string key in this.tabu.artistsTracks.Keys) 
                this.tabu.artistsTracks[key].Sort((a,b) => b.popularity.CompareTo(a.popularity)); 
            
        }

    }
}
