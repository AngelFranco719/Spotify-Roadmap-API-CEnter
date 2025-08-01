using SpotifyRequestManagement.Models.Queries_Models;
using SpotifyRequestManagement.Models.Simplified_Entities;

namespace SpotifyRequestManagement.Models.Multiple_Entities
{
    public class Albums : MultipleObjects, QueryResultInterface<SimplifiedAlbum>
    {
        public SimplifiedAlbum[] items { get; set; }

        public SimplifiedAlbum[] GetResult()
        {
            return items; 
        }

        public override string ToString()
        {
            return items[0].name; 
        }
    }
}
