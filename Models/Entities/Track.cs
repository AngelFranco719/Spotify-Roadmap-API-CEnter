namespace SpotifyRequestManagement.Models.Entities
{
    public class Track : SpotifyBaseObject
    {
        public Album? album { get; set; }
        public SimplifiedArtist[] artists { get; set; }
        public string[] available_markets { get; set; }
        public int disc_number { get; set; }
        public int duration_ms { get; set; }
        public bool @explicit { get; set; }
        public External_IDs external_ids { get; set; }
        public bool is_playable { get; set; }
        public Track linked_from { get; set; }
        public Restriction? restrictions { get; set; }
        public int popularity { get; set; }
        public string? preview_url { get; set; }
        public int track_number { get; set; }
        public bool is_local { get; set; }
    }
}
