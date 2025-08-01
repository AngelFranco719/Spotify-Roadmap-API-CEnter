using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Models.Entities;

namespace SpotifyRequestManagement.Services
{
    public class SpotifyApiRequest
    {
        string URL = "https://api.spotify.com/v1/";
        private readonly HttpClient httpClient;
        private readonly AuthToken token;

        public SpotifyApiRequest(HttpClient _httpClient, AuthToken token)
        {
            httpClient = _httpClient;
            this.token = token;
        }

        public async Task<Artist> getArtist(string id_token)
        {

            string currentURL = URL + $"artists/{id_token}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, currentURL);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            string jsonArtist = await response.Content.ReadAsStringAsync();

            Artist artists = JsonSerializer.Deserialize<Artist>(jsonArtist);

            Console.WriteLine(jsonArtist);

            return artists;

        }

        public async Task<JsonElement> searchByQuery(string encodedQuery) {
            string newQuery = URL + $"search?{encodedQuery}";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, newQuery);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            var response = await httpClient.SendAsync(request); 
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();

            JsonDocument jsonDocument = JsonDocument.Parse(jsonResponse);
            JsonElement root = jsonDocument.RootElement;

            return root; 
        }


        public async Task<RelatedArtists> getRelatedArtist(string id_token)
        {

            string currentURL = URL + $"artists/{id_token}/related-artists";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, currentURL);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            RelatedArtists artists = JsonSerializer.Deserialize<RelatedArtists>(json);

            Console.WriteLine(currentURL);

            return artists;

        }

    }
}
