using SpotifyRequestManagement.Models.Entities;
using System.Collections.Concurrent;
using SpotifyRequestManagement.Models.Simplified_Entities;
using SpotifyRequestManagement.Models;

namespace SpotifyRequestManagement.Services
{
    public class FilterAlgorithmHub
    {
        public Track? root { get; set; }
        public Artist? mainArtist { get; set; }
        public MusicalGenerativeField generativeField;
        public MapSimplifiedTracks mapSimplifiedTracks;
        public TreeFlattener samplerContextualFilter;
        public ConcurrentBag<SimplifiedTrack>? caothicSample;
        public List<Artist> graph = new List<Artist>();
        public Dictionary<string, Artist> refToArtist = new Dictionary<string, Artist>(); 
        public ConcurrentBag<Track> finalCaothicSample;
        public TabuSearch tabu; 
        public DeepFilteringSample deepFilteringSample;
        public ILogger<FilterAlgorithmHub> logger;
        public GetTracksFromArtists getTracksFromArtists;
        public GeneratePlayQueue generatePlayQueue;
        public List<Track> finalQueue; 

        public FilterAlgorithmHub(MusicalGenerativeField generativeField, MapSimplifiedTracks mapSimplifiedTracks,
                                  TreeFlattener samplerContextualFilter, ILogger<FilterAlgorithmHub> logger, 
                                  DeepFilteringSample deepFilteringSample, TabuSearch tabu, 
                                  GetTracksFromArtists getTracksFromArtists, GeneratePlayQueue generatePlayQueue)
        {
            this.generativeField = generativeField;
            this.mapSimplifiedTracks = mapSimplifiedTracks;
            this.samplerContextualFilter = samplerContextualFilter;
            this.logger = logger;
            this.deepFilteringSample = deepFilteringSample;
            this.tabu = tabu;
            this.getTracksFromArtists = getTracksFromArtists;
            this.generatePlayQueue = generatePlayQueue; 
        }

        public async Task MainThread()
        {
            // Obtener el grafo de artistas
            await this.getGenerativeField();
            // Aplanar el grafo 
            await this.setContextualFilter();
            // Obtener información general
            this.getTracksFromArtists.flattenedGraph = this.graph;
            caothicSample = await this.getTracksFromArtists.getTracks();

            // Obtener el campo caótico con información detallada
            await this.setMapSimplifiedTracks();
            // Mapear y preparar información para el filtrado profundo
            mapTracksToDictionary();
            this.tabu.refToArtist = this.refToArtist;
            // Filtrado 
            this.deepFilteringSample.root = this.root;
            this.deepFilteringSample.graph = this.graph;
            this.deepFilteringSample.tabuSearch = this.tabu;
            this.deepFilteringSample.mainThread();
            //Generar la lista de reproducción
            foreach(string key in  this.generatePlayQueue.filteredTracks.Keys) {
                List<Track> current = this.generatePlayQueue.filteredTracks[key];
            }


            this.finalQueue = this.generatePlayQueue.generatePlayQueue(root, 30);

            foreach (Track track in finalQueue) {
                if (track != null)
                    logger.LogInformation("En lista: {name}", track.name); 
            }
                
                

        }

        public async Task getGenerativeField() {
            this.generativeField.root = this.root;
            this.mainArtist = await this.generativeField.getMainArtist();
            this.caothicSample = await this.generativeField.mainThread(); 
        }

        public async Task setContextualFilter() {
            await samplerContextualFilter.setRoot(mainArtist);
            this.graph = this.samplerContextualFilter.getBFS();
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
