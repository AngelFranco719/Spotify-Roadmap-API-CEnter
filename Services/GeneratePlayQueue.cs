using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Models.Entities;

namespace SpotifyRequestManagement.Services
{
    public class GeneratePlayQueue
    {
        public Track root; 
        public Queue<string> familiarArtists = new Queue<string>();
        public Queue<string> diverseArtists = new Queue<string>();
        public Dictionary<string, List<Track>> filteredTracks = new Dictionary<string, List<Track>>(); 
        public int total_duration { get; set; }
        private int expected_size; 
        public List<Track> final_playQueue;
        private int Count = 0;
        double familiarity = 0.70;
        List<char> listReferences; 
        Random random = new Random();
        List<int> FamiliarIndex = new List<int>();
        List<int> DiversityIndex = new List<int>();
        ILogger<GeneratePlayQueue> logger;


        public GeneratePlayQueue(ILogger<GeneratePlayQueue> logger) {
            this.logger = logger; 
        }
        

        public List<Track> generatePlayQueue(Track root, int total_duration)
        {
            this.root = root;
            this.calcTotalTracks(total_duration);
            this.initReferences();
            this.setTracksToPlaylist();

            return this.final_playQueue; 
        }

        public void calcTotalTracks(int total_duration) {
            int average_track_duration = 3;
            this.total_duration = total_duration; 
            this.expected_size= total_duration / average_track_duration;
            final_playQueue = new List<Track>();
            listReferences = new List<char>();

            for (int i = 0; i < expected_size; i++) {
                final_playQueue.Add(null);
                listReferences.Add(' '); 
            }
        } 

        public void initReferences() {

            logger.LogInformation("La queue de familiaridad tiene {num} artistas", familiarArtists.Count);
            logger.LogInformation("La queue de diversidad tiene {num} artistas", diverseArtists.Count); 

            for (int i = 1; i < expected_size; i++) {
                double random_number = random.NextDouble();
                if (random_number <= .70)
                {
                    FamiliarIndex.Add(i);
                    listReferences[i] = 'F';
                    logger.LogInformation("El indice {ind} esta en familiaridad", i); 
                }
                else { 
                    DiversityIndex.Add(i);
                    listReferences[i] = 'D';
                    logger.LogInformation("El indice {ind} esta en diversidad", i);
                }
            }   
        }

        private void setTracksToPlaylist() {

            logger.LogInformation("Las colas tienen: Familiaridad - {f}, Diversidad {d}", familiarArtists.Count, diverseArtists.Count); 

            int initialRepresentation = (int)(expected_size * 0.40);

            final_playQueue[0] = root;
            filteredTracks[root.artists[0].id].RemoveAt(0); 

            while (DiversityIndex.Count != 0) {
             
                string currentID = diverseArtists.Dequeue();
                int randIndex = random.Next(DiversityIndex.Count);

                if (filteredTracks[currentID].Count == 0) 
                    continue;

                try
                {
                    final_playQueue[DiversityIndex[randIndex]] = filteredTracks[currentID][0];
                    DiversityIndex.RemoveAt(randIndex); 
                    filteredTracks[currentID].RemoveAt(0);
                    diverseArtists.Enqueue(currentID);
               
                }
                catch(Exception e)
                {
                    logger.LogError(e.Message); 
                    logger.LogWarning("No hay artistas en diversidad, el tamaño de la cola es {size}", diverseArtists.Count);
                }
                
            }

            string current = familiarArtists.Dequeue();
            double currentPercentaje = .30;
            int nec = (int) (FamiliarIndex.Count * currentPercentaje);
            int agregados = 0; 
            while (FamiliarIndex.Count != 0 && agregados < nec) {
                try
                {
                    int randIndex = random.Next(FamiliarIndex.Count);
                    final_playQueue[FamiliarIndex[randIndex]] = filteredTracks[current][0];
                    FamiliarIndex.RemoveAt(randIndex); 
                    filteredTracks[current].RemoveAt(0);
                    agregados++; 

                    if (agregados == nec)
                    {
                        familiarArtists.Enqueue(current); 
                        current = familiarArtists.Dequeue();
                        nec = Math.Max(1, (int)(FamiliarIndex.Count * currentPercentaje));
                        agregados = 0; 
                    }
                }
                catch(Exception e) {
                    logger.LogError(e.Message); 
                    logger.LogWarning("No hay artistas en Familiaridad, el tamaño de la cola es {size}", familiarArtists.Count);
                    agregados = nec; 
                }
             }
                

        }

    }
}
