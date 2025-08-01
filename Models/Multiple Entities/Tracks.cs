using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models.Multiple_Entities;
using SpotifyRequestManagement.Models.Queries_Models;

namespace SpotifyRequestManagement.Models
{
    public class Tracks : MultipleObjects, QueryResultInterface<Track>
    {
        public Track[] items { get; set; }

        public Track[] GetResult()
        {
            return items; 
        }
    }
}
