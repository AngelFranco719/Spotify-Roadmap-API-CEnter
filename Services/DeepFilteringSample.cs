using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Models.Entities;

namespace SpotifyRequestManagement.Services
{
    public class DeepFilteringSample
    {
        TabuSearch tabuSearch;
        List<Artist> graph;
        int total = 0;
        List<Track> filteredSample = new List<Track>(); 

        public DeepFilteringSample(TabuSearch tabuSearch, List<Artist> graph) {
            this.tabuSearch = tabuSearch;
            this.graph = graph;
            this.total = graph.Count; 
        }

        string setPriority() {
            return tabuSearch.getPriority(); 
        }

        public void mainThread()
        {
            while (filteredSample.Count <= 100) {
                if (setPriority() == "familiarity")
                {

                }
                else { 
                
                }
            }
        }





        


    }
}
