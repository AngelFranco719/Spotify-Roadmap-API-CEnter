using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models.Multiple_Entities;
using SpotifyRequestManagement.Models.Queries_Models;
using SpotifyRequestManagement.Models.Simplified_Entities;

namespace SpotifyRequestManagement.Models
{
    public class Tracks : MultipleObjects, QueryResultInterface<SimplifiedTrack>
    {
        public SimplifiedTrack[] items { get; set; }

        public SimplifiedTrack[] GetResult()
        {
            return items; 
        }
    }
}
