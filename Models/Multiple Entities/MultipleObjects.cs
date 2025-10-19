using SpotifyRequestManagement.Models.Entities;

namespace SpotifyRequestManagement.Models.Multiple_Entities
{
    public class MultipleObjects
    {
        public string href { get; set; }
        public int limit { get; set; }
        public string? next; 
        public int offset { get; set; }
        public string? previous { get; set; }
        public int total { get; set; }

    }
}
