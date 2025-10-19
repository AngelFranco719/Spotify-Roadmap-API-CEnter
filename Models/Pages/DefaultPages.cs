using SpotifyRequestManagement.Models.Simplified_Entities;

namespace SpotifyRequestManagement.Models.Pages
{
    public class DefaultPages
    {
        public string href { get; set; }
        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public string previous { get; set; }
        public int total { get; set; }
    }
}
