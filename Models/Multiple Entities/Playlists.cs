using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models.Queries_Models;
using SpotifyRequestManagement.Models.Simplified_Entities;

namespace SpotifyRequestManagement.Models.Multiple_Entities
{
    public class Playlists : MultipleObjects, QueryResultInterface<SimplifiedPlaylists>
    {

        public SimplifiedPlaylists[] items { get; set; }

        public SimplifiedPlaylists[] GetResult()
        {
            return items; 
        }

    }
}
