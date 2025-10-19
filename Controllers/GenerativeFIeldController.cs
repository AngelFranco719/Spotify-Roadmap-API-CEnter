using Microsoft.AspNetCore.Mvc;
using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Services;

namespace SpotifyRequestManagement.Controllers
{
    [Route("Api/generative-field")]
    [ApiController]
    public class GenerativeFIeldController : Controller
    {
        private readonly SpotifyApiRequest requestController;
        private readonly FilterAlgorithmHub filterAlgorithmHub;
        
        public GenerativeFIeldController(SpotifyApiRequest requestController, FilterAlgorithmHub filterAlgorithmHub)
        {
            this.requestController = requestController;
            this.filterAlgorithmHub = filterAlgorithmHub; 
        }

        [HttpGet]
        public async Task<IActionResult> GetRoot(string id_track) {
            Track request = await requestController.getTrack(id_track);
            filterAlgorithmHub.root = request;
            await filterAlgorithmHub.MainThread();
            return Ok(request);
        }

    }
}
