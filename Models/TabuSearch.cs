using SpotifyRequestManagement.Models.Entities; 

namespace SpotifyRequestManagement.Models
{
    public class TabuSearch
    {
        public Dictionary<string, int> artistsFrecuency = new Dictionary<string, int>();
        public Dictionary<string, int> genresFrecuency = new Dictionary<string, int>();
        public Dictionary<string, List<Track>> artistsTracks = new Dictionary<string, List<Track>>();
        public Dictionary<string, List<Track>> genresTracks = new Dictionary<string, List<Track>>();
        public enum Priority { familiarity, diversity };
        public Priority currentPriority { get; set; } = Priority.familiarity;
        public Dictionary<string, Artist>? refToArtist { get; set; }
        public int total_tracks = 0; 

        public 

        public string getPriority() {
            float calc = 0; 
            foreach (string key in artistsFrecuency.Keys) {
                float total_tracks_artist = artistsFrecuency[key];
                float popularity = refToArtist[key].popularity / 100f;
                calc += (total_tracks_artist * popularity) / (float)total_tracks; 
            }

            if (calc < .70)
                currentPriority = Priority.familiarity;
            else
                currentPriority = Priority.diversity;

            return currentPriority.ToString(); 
        }
    }
}
