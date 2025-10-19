using System.Collections.Concurrent;
using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models.Multiple_Entities;
using SpotifyRequestManagement.Models.Simplified_Entities; 

namespace SpotifyRequestManagement.Services
{
    public class MapSimplifiedTracks
    {
        public ConcurrentBag<SimplifiedTrack>? initialSample { get; set;}
        List<List<string>> requestGroups = new List<List<string>>();
        ConcurrentBag<Track> finalSample = new ConcurrentBag<Track>();
        SpotifyApiRequest apiRequest;
        ILogger<MapSimplifiedTracks> logger; 

        public MapSimplifiedTracks(SpotifyApiRequest _apiRequest, ILogger<MapSimplifiedTracks> logger) {
            this.apiRequest = _apiRequest;
            this.logger = logger; 
        }

        public async Task<ConcurrentBag<Track>> setGroups() {
            List<string> newGroup = new List<string>();

            foreach (SimplifiedTrack simplifiedTrack in initialSample) {
                if (newGroup.Count < 50)
                    newGroup.Add(simplifiedTrack.id);
                else {
                    requestGroups.Add(newGroup);
                    newGroup = new List<string> { simplifiedTrack.id};
                }
            }
            requestGroups.Add(newGroup);
            await getFinalSample();
            return this.finalSample; 
        }

        private async Task getFinalSample() {
            List<Task> concurrence = new List<Task>();
            foreach (List<string> group in requestGroups)
                concurrence.Add(requestTracks(group));
            await Task.WhenAll(concurrence); 
        }

        private async Task requestTracks(List<string> group) {
            SeveralTracksResponse severalTracksResponse = await apiRequest.getSeveralTracks(group);
            foreach (Track track in severalTracksResponse.tracks)
                finalSample.Add(track); 
        }


    }
}
