using System.Text.Json.Serialization; 

namespace SpotifyRequestManagement.Models.Entities
{
    public class FMArtist
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("mbid")]
        public string Mbid { get; set; }

        [JsonPropertyName("match")]
        public string Match { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("image")]

        public List<FMImage> FMImage { get; set; }

    }

    public class FMImage
    {
        [JsonPropertyName("#text")]
        public string Url { get; set; }

        [JsonPropertyName("size")]
        public string Size { get; set; }
    }
}
