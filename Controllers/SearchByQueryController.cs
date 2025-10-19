using Microsoft.AspNetCore.Mvc;
using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models.Multiple_Entities;
using SpotifyRequestManagement.Models.Simplified_Entities;
using SpotifyRequestManagement.Services;

namespace SpotifyRequestManagement.Controllers
{
    [Route("api/Search/[controller]")]
    [ApiController]
    public class SearchByQueryController : Controller
    {
        private readonly SpotifyAuthService spotifyAuthService;
        private readonly SpotifyApiRequest spotifyApiRequest;

        public SearchByQueryController(SpotifyAuthService spotifyAuthService, SpotifyApiRequest spotifyApiRequest)
        {
            this.spotifyAuthService = spotifyAuthService;
            this.spotifyApiRequest = spotifyApiRequest;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuery([FromQuery] QuerySearch query) {
            QueryResponse queryResponse = new QueryResponse(query, spotifyApiRequest);

            string type = query.type[0];
            query.type = []; 

            Console.WriteLine(queryResponse.encodedQuery); 

            switch (type) {

                case "track":
                    var resultTrack = await queryResponse.getQueryResponse<SimplifiedTrack,Tracks>();
                    var tracks = resultTrack.GetResult();
                    return Ok(tracks);

                case "artist":
                    var resultArtist = await queryResponse.getQueryResponse<Artist, Artists>(); 
                    var artists = resultArtist.GetResult();
                    return Ok(artists);

                case "playlist":
                    var resultPlaylist = await queryResponse.getQueryResponse<SimplifiedPlaylists, Playlists>(); 
                    var playlist = resultPlaylist.GetResult();
                    return Ok(playlist);

                case "album":
                    var resultAlbum = await queryResponse.getQueryResponse<SimplifiedAlbum, Albums>();
                    var album = resultAlbum.GetResult();
                    return Ok(album);

                default:
                    return BadRequest("Tipo No Soportado"); 

            
            }
        }
    }
}
