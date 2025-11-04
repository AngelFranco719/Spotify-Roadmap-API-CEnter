using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Models.Entities;

namespace SpotifyRequestManagement.Services
{
    public class DeepFilteringSample
    {
        public TabuSearch tabuSearch { get; set; }
        public List<Artist> graph { get; set; }
        int total = 0;
        List<Track> filteredSample = new List<Track>();
        public Track root { get; set; }
        int iteration = 0;
        ILogger<DeepFilteringSample> logger; 

        public DeepFilteringSample(TabuSearch tabuSearch, ILoggerFactory factory)
        {
            this.tabuSearch = tabuSearch;
            this.logger = factory.CreateLogger<DeepFilteringSample>();
        }

        string setPriority() {
            return tabuSearch.getPriority(); 
        }

        public void mainThread()
        {
            this.total = graph.Count;
            this.filteredSample.Add(root); // <- Agrego la raíz a la muestra curada.
            this.tabuSearch.generatePlayQueue.familiarArtists.Enqueue(root.artists[0].id); 
            this.tabuSearch.historial.Enqueue(root);
            this.tabuSearch.generatePlayQueue.filteredTracks.Add(root.artists[0].id, new List<Track> { root }); 

            this.tabuSearch.artistsFrecuency.Add(root.artists[0].id, 1); 
            
            iteration++; 

            while (filteredSample.Count <= 100) {

                logger.LogInformation("Estoy trabajando en tabu");

                string priority = setPriority();
                int ini = priority == "familiarity" ? 0 : (graph.Count / 2) + 1;
                int fin = graph.Count; 

                for (int i = ini; i < fin; i++)
                {
                    if (this.tabuSearch.tabuList.ContainsKey(graph[i].id)) {
                        if (this.tabuSearch.tabuList[graph[i].id] >= iteration)                       
                            continue;
                        else {
                            this.tabuSearch.tabuList[graph[i].id] = 0;
                            this.tabuSearch.countIterations[graph[i].id] = 0; 
                        }
                    }
                    else
                        this.tabuSearch.tabuList[graph[i].id] = 0;

                    Track? track = this.tabuSearch.addTrack(graph[i].id, iteration, i, graph.Count);
                    if (track != null) {
                        this.filteredSample.Add(track);
                        break;
                    }
                }
                iteration++; 
            }
            foreach (Track track in filteredSample) {
                logger.LogInformation("El Track {name} es del artista {artist}", track.name, track.artists[0].name); 
            }
        }





        


    }
}
