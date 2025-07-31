using System.Text.Json.Serialization;

namespace SpotifyRequestManagement.Models
{
    public class Album : SpotifyBaseObject
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum AlbumTypes
        {
            album, 
            single, 
            compilation
        }

        public AlbumTypes album_type { get; set; }
        public int total_tracks; 
        public string[] available_markets { get; set; }
        public Image[] images { get; set; }
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        public Restriction restrictions { get; set; }
        public SpotifyBaseObject[] artist; 

    }
}
