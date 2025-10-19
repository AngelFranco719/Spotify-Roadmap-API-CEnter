using Microsoft.AspNetCore.Mvc;
using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Services;

namespace SpotifyRequestManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : Controller
    {
        private readonly SpotifyApiRequest requestController;

        public AlbumController(SpotifyApiRequest requestController) { 
            this.requestController = requestController;
        }

        [HttpGet]
        public async Task<IActionResult> GetAlbumByID(string ID) {

            Album request = await requestController.getAlbum(ID); 

            return Ok(request); 
        }

        [HttpGet("Tracks/{ID}")]
        public async Task<IActionResult> GetAlbumTracks(string ID) {

            Tracks response = await requestController.getAlbumTracks(ID); 

            return Ok(response); 
        }

        [HttpGet("several")]
        public async Task<IActionResult> GetSeveralAlbums() {

            return Ok(); 
        }

    }
}
