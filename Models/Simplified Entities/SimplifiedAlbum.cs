using SpotifyRequestManagement.Models.Entities;

namespace SpotifyRequestManagement.Models.Simplified_Entities
{
    public class SimplifiedAlbum : SpotifyBaseObject
    {
        public string album_type {  get; set; }
        public int total_tracks { get; set; }
        public string[] available_markets { get; set; }
        public Image[] images { get; set; }
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        public Restriction restrictions { get; set; }
        public SimplifiedArtist[] artists { get; set; }
    }
}
