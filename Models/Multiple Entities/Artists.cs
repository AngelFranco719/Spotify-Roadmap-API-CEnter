using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models.Queries_Models;

namespace SpotifyRequestManagement.Models.Multiple_Entities
{
    public class Artists : MultipleObjects, QueryResultInterface<Artist>
    {
        public Artist[] items { get; set; }

        public Artist[] GetResult()
        {
            return items; 
        }
    }
}
