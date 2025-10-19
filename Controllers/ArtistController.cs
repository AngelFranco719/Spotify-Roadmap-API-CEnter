using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Services;

namespace SpotifyRequestManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : Controller
    {
        private readonly SpotifyApiRequest requestController;

        public ArtistController(SpotifyApiRequest _requestController) {

            requestController = _requestController; 

        }

        [HttpGet]
        // GET: ArtistController
        public async Task<IActionResult> Get(string id_token) { 

            Artist request = await requestController.getArtist(id_token);

            return (Ok(request)); 
            
        }


    }
}
