using Microsoft.AspNetCore.Mvc;
using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models.Multiple_Entities;
using SpotifyRequestManagement.Models.Simplified_Entities;
using SpotifyRequestManagement.Services;

namespace SpotifyRequestManagement.Controllers
{
    public class SearchByQueryController
    {
        private readonly SpotifyAuthService spotifyAuthService;
        private readonly SpotifyApiRequest spotifyApiRequest;

        public SearchByQueryController(SpotifyAuthService spotifyAuthService, SpotifyApiRequest spotifyApiRequest)
        {
            this.spotifyAuthService = spotifyAuthService;
            this.spotifyApiRequest = spotifyApiRequest;
        }

        public async Task<SimplifiedTrack[]> getTracksByQuery(QuerySearch query) {
            QueryResponse queryResponse = new QueryResponse(query, spotifyApiRequest);
            var resultTrack = await queryResponse.getQueryResponse<SimplifiedTrack, Tracks>();
            var tracks = resultTrack.GetResult();
            return tracks;

        }
    }
}
