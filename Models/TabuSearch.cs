using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Services;

namespace SpotifyRequestManagement.Models
{
    public class TabuSearch
    {
        public Dictionary<string, int> artistsFrecuency = new Dictionary<string, int>();
        public Dictionary<string, int> genresFrecuency = new Dictionary<string, int>();
        public Dictionary<string, List<Track>> artistsTracks = new Dictionary<string,List<Track>>();
        public Dictionary<string, List<Track>> genresTracks = new Dictionary<string, List<Track>>();
        public enum Priority { familiarity, diversity };
        public Priority currentPriority { get; set; } = Priority.familiarity;
        public Dictionary<string, Artist>? refToArtist { get; set; }
        public int total_tracks = 1;
        public Dictionary<string, int> tabuList = new Dictionary<string, int>();
        public Dictionary<string, int> countIterations = new Dictionary<string, int>(); 
        public static int tabuFamiliaritylimit = 5;
        private readonly ILogger<TabuSearch> logger;
        private static Random random = new Random();
        public Queue<Track> historial = new Queue<Track>();
        public GeneratePlayQueue generatePlayQueue; 

        public TabuSearch(ILoggerFactory loggerFactory, GeneratePlayQueue generatePlayQueue) {
            this.logger = loggerFactory.CreateLogger<TabuSearch>(); 
            this.generatePlayQueue = generatePlayQueue; 
        }

        Dictionary<string, int> getTemp(){
            Queue<Track> aux = new Queue<Track>(historial);
            Dictionary<string, int> temp = new Dictionary<string, int>(); 
            while (aux.Count != 0) {
                Track actual = aux.Dequeue();
                if (temp.TryGetValue(actual.artists[0].id, out int count))
                    temp[actual.artists[0].id] = count + 1;
                else
                    temp[actual.artists[0].id] = 1; 
            }
            return temp; 
        }
        public string getPriority() {
            double calc = 0;
            Dictionary<string, int> temp = this.getTemp() ; 
            foreach (string key in temp.Keys) 
                calc += Math.Pow(temp[key], 2);

            calc /= Math.Pow(historial.Count, 2); 

            if (calc <= .45)
                currentPriority = Priority.familiarity;
            else
                currentPriority = Priority.diversity;

            return currentPriority.ToString();
        }

        public Track? addTrack(string id_artist, int current_iteration, int index_graph, int total_graph) {
            total_tracks++;
            if (this.artistsFrecuency.TryGetValue(id_artist, out int freq))
                this.artistsFrecuency[id_artist]++;
            else {
                this.artistsFrecuency[id_artist] = 1;
                if (this.currentPriority.ToString() == "diversity") 
                    this.generatePlayQueue.diverseArtists.Enqueue(id_artist);
                else
                    this.generatePlayQueue.familiarArtists.Enqueue(id_artist);
            }
                

            if (this.countIterations.TryGetValue(id_artist, out int ite))
                this.countIterations[id_artist]++;
            else
                this.countIterations[id_artist] = 0; 

            logger.LogInformation("El artista actual tiene {rep}", this.artistsFrecuency[id_artist]); 

            if (this.artistsFrecuency[id_artist] > tabuFamiliaritylimit)
                this.tabuList[id_artist] = current_iteration + 20;

            if (this.currentPriority.ToString() == "diversity")
                    this.tabuList[id_artist] = current_iteration + 30;
            try
            {
                int maxRan = total_graph / 2 >= index_graph ? 10 : 30;
                if (!this.artistsTracks.ContainsKey(id_artist) || this.artistsTracks[id_artist].Count == 0)
                {
                    logger.LogWarning("Artista {id} no tiene canciones registradas en artistsTracks", id_artist);
                    this.tabuList[id_artist] = current_iteration + 10000;
                    return null;
                }
                var ran = this.artistsTracks[id_artist].Take(maxRan).ToList();
                Track song = ran[random.Next(ran.Count)];
                this.artistsTracks[id_artist].Remove(song); 
                historial.Enqueue(song);

                if (historial.Count >= 50)
                    historial.Dequeue();

                if (this.generatePlayQueue.filteredTracks.TryGetValue(id_artist, out var lista))
                    this.generatePlayQueue.filteredTracks[id_artist].Add(song);
                else
                    this.generatePlayQueue.filteredTracks[id_artist] = new List<Track> { song }; 
                

                return song;
            }
            catch (Exception ex)
            {
                logger.LogWarning("El artista se quedó sin canciones");
                this.tabuList[id_artist] += 10000; 
                return null; 
            }
        }
    }
}
