using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
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

        public MapSimplifiedTracks(SpotifyApiRequest _apiRequest, ILoggerFactory loggerFactory) {
            this.apiRequest = _apiRequest;
            this.logger = loggerFactory.CreateLogger<MapSimplifiedTracks>();
        }

        public async Task<ConcurrentBag<Track>> setGroups() {
            List<string> newGroup = new List<string>();

            HashSet<string> searched = new HashSet<string>();

            foreach (SimplifiedTrack simplifiedTrack in initialSample) {

                if (!searched.Add(simplifiedTrack.name))
                    continue; 

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

            int maxConcurrentRequest = 2;
            int delay = 1000;
            SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrentRequest);

            foreach (List<string> request in requestGroups) {
                await semaphore.WaitAsync();
                concurrence.Add(Task.Run(async () => {
                    try
                    {
                        await requestTracks(request); 
                    }
                    catch(Exception e)
                    {
                        logger.LogError(e.Message); 
                    }
                    finally
                    {
                        await Task.Delay(delay);
                        semaphore.Release(); 
                    }
               }));         
            }

            await Task.WhenAll(concurrence);          
        }

        private async Task requestTracks(List<string> group) {
            SeveralTracksResponse severalTracksResponse = await apiRequest.getSeveralTracks(group);
            foreach (Track track in severalTracksResponse.tracks)
                finalSample.Add(track); 
        }


    }
}
