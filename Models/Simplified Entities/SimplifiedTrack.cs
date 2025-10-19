using SpotifyRequestManagement.Models.Entities;

namespace SpotifyRequestManagement.Models.Simplified_Entities
{
    public class SimplifiedTrack : SpotifyBaseObject
    {
        public SimplifiedArtist[] artists { get; set; }
        public string[] available_markets { get; set; }
        public int disc_number { get; set; }
        public int duration_ms { get; set; }
        public bool @explicit {get; set; }
        public ExternalURLS external_urls { get; set; }
        public bool is_playable { get; set; }
        public LinkedFrom linked_from { get; set; }
        public Restriction restriction { get; set; }
        public int track_number { get; set; }
        public bool is_local { get; set; }
        public External_IDs external_ids { get; set; }
        public string[] genres { get; set; }
        public string label { get; set; }
        int popularity { get; set; }

        public override string ToString()
        {
            return $"------------------------------" +
                $"\n\nCancion: {this.name}\n " +
                $"Popularidad: {this.popularity}" +
                $"------------------------------"
                ; 
        }
    }
}
