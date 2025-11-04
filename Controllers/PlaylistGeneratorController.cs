using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Services;

namespace SpotifyRequestManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistGeneratorController : Controller
    {
        SpotifyApiRequest spotifyApiRequest;
        ILoggerFactory loggerFactory;
        LastFMApiRequest lastFMApiRequest;
        private readonly IHubContext<ProgressHub> _hubContext;
        private readonly AuthToken auth; 
        ILogger<PlaylistGeneratorController> logger;    

        public PlaylistGeneratorController(SpotifyApiRequest spotifyApiRequest,
            ILoggerFactory loggerFactory, LastFMApiRequest lastFMApiRequest,
            IHubContext<ProgressHub> _hubContext, AuthToken auth, 
            ILogger<PlaylistGeneratorController> logger)
        {
            this.spotifyApiRequest = spotifyApiRequest;
            this.loggerFactory = loggerFactory;
            this.lastFMApiRequest = lastFMApiRequest;
            this._hubContext = _hubContext;
            this.auth = auth; 
            this.logger = logger; 
        }

        [HttpPost]
        public async Task<IActionResult> GetPlaylist([FromBody] List<Station> roots, [FromQuery] string connectionID)
        {

            List<List<Track>> playlist = new List<List<Track>>();
            foreach (var root in roots)
            {
                FilterAlgorithmHub filterAlgorithmHub = new FilterAlgorithmHub(spotifyApiRequest, loggerFactory, lastFMApiRequest, _hubContext, connectionID);
                List<Track> nueva;
                filterAlgorithmHub.duration = root.duration;
                nueva = await filterAlgorithmHub.MainThread(root.key);
                playlist.Add(nueva);
            }

            List<Track> FinalPlaylist = new List<Track>(); 

            foreach (List<Track> pl in playlist) {
                pl.ForEach((element) =>
                {
                    FinalPlaylist.Add(element);
                }); 
            }
            return Ok(FinalPlaylist); 
        }

        [HttpPost("/api/getPlaylistUrl")]
        public async Task<IActionResult> getPlaylistURL([FromBody] List<string> playlist)
        {
            try {
                string token = this.auth.user_token; 
                logger.LogInformation("Token: {token}", token); 
                string playlistUrl = await spotifyApiRequest.createPlaylist(playlist, token);
                return (Ok(playlistUrl)); 
            } catch (Exception e) {
                logger.LogInformation(e.Message); 
                return (StatusCode(500, $"Error interno {e.Message}")); 
            }
        }
    }
}
