using Microsoft.AspNetCore.SignalR;
using SpotifyRequestManagement.Models.Entities;

namespace SpotifyRequestManagement.Services
{
    public class ProgressHub : Hub
    {
        public async Task sendFeedback(List<Artist> artist) { 
            await Clients.All.SendAsync("getFeedback", artist);
        }

        public async Task sendUpdateSongsFound(int num)
        {
            await Clients.All.SendAsync("updateSongsFound", num); 
        }
    }
}
