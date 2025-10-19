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
        public SamplerContextualFilter samplerContextualFilter;
        public ConcurrentBag<SimplifiedTrack>? caothicSample;
        public List<Artist> graph = new List<Artist>();
        public Dictionary<string, Artist> refToArtist = new Dictionary<string, Artist>(); 
        public ConcurrentBag<Track> finalCaothicSample;
        public TabuSearch tabu = new TabuSearch();
        public ILogger<FilterAlgorithmHub> logger; 

        public FilterAlgorithmHub(MusicalGenerativeField generativeField, MapSimplifiedTracks mapSimplifiedTracks,
                                  SamplerContextualFilter samplerContextualFilter, ILogger<FilterAlgorithmHub> logger)
        {
            this.generativeField = generativeField;
            this.mapSimplifiedTracks = mapSimplifiedTracks;
            this.samplerContextualFilter = samplerContextualFilter;
            this.logger = logger; 
        }

        public async Task MainThread()
        {
            // Obtener el campo caótico de canciones.
            await this.getGenerativeField();
            // Aplanar el grafo 
            await this.setContextualFilter();
            // Obtener el campo caótico con información detallada
            await this.setMapSimplifiedTracks();
            // Mapear y preparar información para el filtrado profundo
            mapTracksToDictionary();
            this.tabu.refToArtist = this.refToArtist;
        }

        public async Task getGenerativeField() {
            this.generativeField.root = this.root;
            this.mainArtist = await this.generativeField.getMainArtist();
            caothicSample = await this.generativeField.mainThread(); 
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
                string[] genres = refToArtist[artist_id].genres; 
                if (!this.tabu.artistsTracks.TryGetValue(artist_id, out var listTrack))
                {
                    listTrack = new List<Track>();
                    this.tabu.artistsTracks[artist_id] = listTrack;
                }

                foreach (string genre in genres) {
                    if (!this.tabu.genresTracks.TryGetValue(genre, out var list)) {
                        list = new List<Track>();
                        this.tabu.genresTracks[genre] = list; 
                    }
                    list.Add(item); 
                }
                listTrack.Add(item);
            }
        }

    }
}
