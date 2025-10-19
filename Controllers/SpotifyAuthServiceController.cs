using Microsoft.AspNetCore.Mvc;
using SpotifyRequestManagement.Services;

namespace SpotifyRequestManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyAuthServiceController : ControllerBase
    {

        private readonly SpotifyAuthService spotifyAuthService; 

        public SpotifyAuthServiceController(SpotifyAuthService _spotifyAuthService) { 
            spotifyAuthService = _spotifyAuthService;
        } 

        // GET: api/<SpotifyAuthServiceController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var token = await spotifyAuthService.getResponse();

            return (Ok(new { request_token = token})); 
        }

        [HttpGet("Callback")]
        public async Task<IActionResult> Callback([FromQuery] string code) {
            string redirectUri = "http://127.0.0.1:7261/callback";
            var token = await spotifyAuthService.ExchangeCodeForToken(code, redirectUri);
            spotifyAuthService.SetOAuthToken(token.access_token); 
            return Ok(token); 
        }
    }
}
