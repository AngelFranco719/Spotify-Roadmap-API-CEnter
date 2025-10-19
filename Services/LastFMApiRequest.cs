using SpotifyRequestManagement.Models.Entities;
using System.Text.Json; 


namespace SpotifyRequestManagement.Services
{
    public class LastFMApiRequest
    {
        private readonly string URL = $"http://ws.audioscrobbler.com/2.0/";
        private readonly string api_key = "e2f3a68e9dfce36164d2829d676f807b";
        private readonly HttpClient httpClient; 


        public LastFMApiRequest(HttpClient _httpClient) {
            this.httpClient = _httpClient; 
        }

        public async Task<SimilarArtists> getSimilarArtists(string artist_name) {
            string currentURL = URL + $"?method=artist.getsimilar&artist={artist_name}&api_key={api_key}&format=json";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, currentURL);
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string jsonSimilarArtists = await response.Content.ReadAsStringAsync();
            SimilarArtistsResponse similarArtistsResponse = JsonSerializer.Deserialize<SimilarArtistsResponse>(jsonSimilarArtists);
            SimilarArtists similarArtists = similarArtistsResponse.similarartists; 
            return similarArtists; 
        }

    }
}
