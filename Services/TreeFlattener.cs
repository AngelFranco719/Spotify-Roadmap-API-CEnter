using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models.Simplified_Entities;
using System.Collections.Concurrent; 

namespace SpotifyRequestManagement.Services
{
    public class TreeFlattener
    {
        Artist? root;
        Queue<Artist> bfs = new Queue<Artist>();
        private readonly ILogger<TreeFlattener> logger;
        List<Artist> graph = new List<Artist>();
        MapSimplifiedTracks mapSimplifiedTracks; 
        public ConcurrentBag<SimplifiedTrack>? currentSample { get; set; }

        public TreeFlattener(ILoggerFactory _logger, MapSimplifiedTracks mapSimplifiedTracks) {
            logger = _logger.CreateLogger<TreeFlattener>();
            this.mapSimplifiedTracks = mapSimplifiedTracks; 
        }

        public async Task setRoot(Artist root) {
            this.root = root;
            addRelatedArtists(root);
            graph.Add(root);  
        }

        public List<Artist> getBFS()
        {
            while (bfs.Count != 0) {
                Artist current = bfs.First();
                bfs.Dequeue();
                addRelatedArtists(current);
                graph.Add(current); 
            }

            return this.graph; 
        }

        private void addRelatedArtists(Artist current) {
            foreach (Artist related in current.relatedArtist) 
                bfs.Enqueue(related);                
        }

    }
}
