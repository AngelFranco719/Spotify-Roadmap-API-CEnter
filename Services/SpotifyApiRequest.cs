using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models.Multiple_Entities;
using SpotifyRequestManagement.Models.Pages;
using SpotifyRequestManagement.Models.Simplified_Entities;

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

        public async Task<Album> getAlbum(string ID)
        {
            string currentURL = URL + $"albums/{ID}";
            Console.WriteLine(currentURL);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, currentURL);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            var response = await httpClient.SendAsync(request); 
            response.EnsureSuccessStatusCode();         
            string jsonAlbum = await response.Content.ReadAsStringAsync();
            Album album = JsonSerializer.Deserialize<Album>(jsonAlbum);
            return album; 
        }

        public async Task<Tracks> getAlbumTracks(string ID) {
            string currentURL = URL + $"albums/{ID}/tracks"; 
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, currentURL );
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode(); 

            string jsonTracks = await response.Content.ReadAsStringAsync();
            Tracks tracks = JsonSerializer.Deserialize<Tracks>(jsonTracks);

            return tracks; 
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

        public async Task<Track> getTrack(string id_track) {
            string currentURL = URL + $"tracks/{id_track}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, currentURL);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync(); 
            Track track = JsonSerializer.Deserialize<Track>(json);

            Console.WriteLine(currentURL); 

            return track; 
        }

        public async Task<AlbumPages> getArtistAlbums(string id_artist) {
            string currentURL = URL + $"artists/{id_artist}/albums?include_groups=album";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, currentURL);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            AlbumPages albumPages = JsonSerializer.Deserialize<AlbumPages>(json);

            return albumPages; 
        }

        public async Task<SeveralTracksResponse> getSeveralTracks(List<string> ids) {
            string currentURL = URL + $"tracks?ids=";
            foreach (string id in ids)
                currentURL += id + ",";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, currentURL);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            SeveralTracksResponse severalTracksResponse = JsonSerializer.Deserialize<SeveralTracksResponse>(json); 

            return severalTracksResponse; 
        }

    }
}
