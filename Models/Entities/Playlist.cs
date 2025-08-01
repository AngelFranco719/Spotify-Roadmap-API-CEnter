namespace SpotifyRequestManagement.Models.Entities
{
    public class Playlist : SpotifyBaseObject
    {

        public bool collaborative {  get; set; }
        public string description { get; set; }
        public Image[] images { get; set; }
        public Owner owner { get; set; }
        public bool @public { get; set; }
        public string snapshot_id { get; set; }
        public Tracks tracks { get; set; }

    }
}
