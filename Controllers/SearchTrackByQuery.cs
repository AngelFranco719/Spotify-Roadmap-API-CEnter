using Microsoft.AspNetCore.Mvc;
using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models.Multiple_Entities;
using SpotifyRequestManagement.Models.Simplified_Entities;
using SpotifyRequestManagement.Services;

namespace SpotifyRequestManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchTrackByQuery : Controller
    {

        private readonly SpotifyApiRequest spotifyApiRequest;
        private SearchByQueryController searchByQueryController;

        public SearchTrackByQuery(SpotifyApiRequest spotifyApiRequest, SearchByQueryController searchByQueryController)
        {
            this.spotifyApiRequest = spotifyApiRequest;
            this.searchByQueryController = searchByQueryController; 
        }

        [HttpGet]
        public async Task<IActionResult> GetTrackByQuery(string query) {
            List<Track> tracks = new List<Track>();
            QuerySearch querySearch = new QuerySearch();

            querySearch.search = query;
            querySearch.type = ["track"];

            SimplifiedTrack[] simplifiedTracks = await searchByQueryController.getTracksByQuery(querySearch);
            List<string> ids = new List<string>();

            foreach (SimplifiedTrack simplified in simplifiedTracks) 
                ids.Add(simplified.id);

            SeveralTracksResponse several = await spotifyApiRequest.getSeveralTracks(ids);
            tracks = several.tracks.ToList();
            return Ok(tracks); 
        }
       
    }
}
