namespace SpotifyRequestManagement.Models.Entities
{
    public class SimilarArtists
    {
        public List<FMArtist> artist { get; set; }
    }

    public class SimilarArtistsResponse
    {
        public SimilarArtists similarartists { get; set; }
    }
}
