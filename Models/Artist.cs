namespace SpotifyRequestManagement.Models
{
    public class Artist : SpotifyBaseObject
    {
        public ExternalURLS external_urls { get; set; }
        public Followers followers { get; set; }
        public string[] genres { get; set; }
        public Image[] images { get; set; }

    }
}
