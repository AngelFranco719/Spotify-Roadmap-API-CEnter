using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models.Queries_Models;

namespace SpotifyRequestManagement.Models.Multiple_Entities
{
    public class Albums : MultipleObjects, QueryResultInterface<Album>
    {
        public Album[] albums { get; set; }

        public Album[] GetResult()
        {
            return albums; 
        }

        public override string ToString()
        {
            return albums[0].name; 
        }
    }
}
