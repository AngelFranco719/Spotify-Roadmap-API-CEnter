using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models.Multiple_Entities;

namespace SpotifyRequestManagement.Models.Simplified_Entities
{
    public class SimplifiedPlaylists : SpotifyBaseObject
    {
        public bool collaborative { get; set; }
        public string description { get; set; }
        public Image[] images { get; set; }
        public Owner owner { get; set; }
        public bool @public { get; set; }
        public string snapshot_id { get; set; }
        public Tracks tracks { get; set; }

    }
}
