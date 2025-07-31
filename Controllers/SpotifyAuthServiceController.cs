using Microsoft.AspNetCore.Mvc;
using SpotifyRequestManagement.Models;

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

        // GET api/<SpotifyAuthServiceController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SpotifyAuthServiceController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SpotifyAuthServiceController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SpotifyAuthServiceController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
