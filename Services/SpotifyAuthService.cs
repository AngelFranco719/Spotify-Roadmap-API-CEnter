using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using SpotifyRequestManagement.Models;
using static System.Net.WebRequestMethods;

namespace SpotifyRequestManagement.Services
{
    public class SpotifyAuthService
    {
        private const string clientID = "84a658a2fd724fd9b69f8b8e8f9874ba";
        private const string clientSecret = "beafe247116747c6a55ac767e0d52d4e";
        private const string URL = "https://accounts.spotify.com/api/token";
        private const string redirectUri = "http://127.0.0.1:7261/callback";
        private const string scope = "playlist-modify-private playlist-modify-public";

        public string userToken { get; set; }

        static FormUrlEncodedContent content = new FormUrlEncodedContent(new[] {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", clientID),
            new KeyValuePair<string, string>("client_secret", clientSecret)
        });

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, URL)
        {
            Content = content
        };

        private readonly HttpClient client;
        private AuthToken authToken;

        public SpotifyAuthService(HttpClient _client, AuthToken _authToken)
        {

            client = _client;
            authToken = _authToken;

        }

        public async Task<string> getRequest()
        {
            var body = await content.ReadAsStringAsync();
            return URL + '?' + body;
        }

        private string getDeserializedJson(string jsonResponse)
        {

            var spotifyToken = JsonSerializer.Deserialize<SpotifyTokenResponse>(jsonResponse);
            authToken.token = spotifyToken.access_token;
            return authToken.token;

        }

        public async Task<string> getResponse()
        {
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            string jsonContent = await response.Content.ReadAsStringAsync();

            return getDeserializedJson(jsonContent);
        }

        public async Task<SpotifyTokenResponse> ExchangeCodeForToken(string code, string redirectUri) {
            var postData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("client_id", clientID),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            });

            var response = await client.PostAsync(URL, postData); 
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<SpotifyTokenResponse>(json); 
        }

        public void SetOAuthToken(string o_token)
        {
            authToken.user_token = o_token;
            return; 
        }
    }
}
