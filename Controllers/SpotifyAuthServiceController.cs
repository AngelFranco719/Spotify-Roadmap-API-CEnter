using Microsoft.AspNetCore.Mvc;
using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Services;

namespace SpotifyRequestManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyAuthServiceController : ControllerBase
    {

        private readonly SpotifyAuthService spotifyAuthService; 
        private readonly ILogger<SpotifyAuthServiceController> logger;
        private readonly AuthToken auth; 

        public SpotifyAuthServiceController(SpotifyAuthService _spotifyAuthService, 
            ILogger<SpotifyAuthServiceController> logger, AuthToken auth)
        {
            spotifyAuthService = _spotifyAuthService;
            this.logger = logger;
            this.auth = auth; 
        }

        // GET: api/<SpotifyAuthServiceController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var token = await spotifyAuthService.getResponse();

            return (Ok(new { request_token = token})); 
        }

        [HttpGet("Callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Missing code.");

            string redirectUri = "http://127.0.0.1:7261/callback";

            var token = await spotifyAuthService.ExchangeCodeForToken(code, redirectUri);

            spotifyAuthService.SetOAuthToken(token.access_token);

            auth.user_token = token.access_token; 

            logger.LogInformation("Obtuve el token {token}", spotifyAuthService.userToken); 

            System.IO.File.WriteAllText("spotify_refresh_token.txt", token.refresh_token ?? "");

            return Ok(token);
        }

    }
}
