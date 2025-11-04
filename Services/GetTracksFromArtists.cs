using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models.Multiple_Entities;
using SpotifyRequestManagement.Models.Pages;
using SpotifyRequestManagement.Models.Simplified_Entities;
using System.Collections.Concurrent;
using System.Threading;

namespace SpotifyRequestManagement.Services
{
    public class GetTracksFromArtists
    {
        public List<Artist>? flattenedGraph { get; set; }
        private readonly SpotifyApiRequest spotifyApiRequest;
        private readonly ILogger<GetTracksFromArtists> logger;
        private readonly IHubContext<ProgressHub> _hubContext;
        private readonly string connectionId;
        private readonly int batchSize = 50;

    private ConcurrentBag<SimplifiedTrack> sample = new ConcurrentBag<SimplifiedTrack>();
        private int totalTracks = 0;

        public GetTracksFromArtists(
            SpotifyApiRequest spotifyApiRequest,
            ILoggerFactory factory,
            IHubContext<ProgressHub> hubContext,
            string connectionId
        )
        {
            this.spotifyApiRequest = spotifyApiRequest;
            this.logger = factory.CreateLogger<GetTracksFromArtists>();
            this._hubContext = hubContext;
            this.connectionId = connectionId;
        }

        public async Task<ConcurrentBag<SimplifiedTrack>> getTracks()
        {
            int maxConcurrentRequest = 2;
            int delay = 1000;
            SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrentRequest);
            List<Task> tasks = new List<Task>();

            foreach (Artist artist in flattenedGraph)
            {
                await semaphore.WaitAsync();
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await ProcessArtistAlbums(artist.id);
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning(e.Message);
                    }
                    finally
                    {
                        await Task.Delay(delay);
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
            return sample;
        }

        private async Task ProcessArtistAlbums(string artistID)
        {
            AlbumPages albumPages = await spotifyApiRequest.getArtistAlbums(artistID);
            await ProcessAlbumPages(albumPages);
        }

        private async Task ProcessAlbumPages(AlbumPages albumPages)
        {
            List<string> albumIds = albumPages.items.Select(a => a.id).ToList();
            Albums albumsResult = await spotifyApiRequest.getAlbums(albumIds);
            List<Album> sortedAlbums = albumsResult.albums.OrderByDescending(a => a.popularity).ToList();

            for (int i = 0; i < Math.Min(5, sortedAlbums.Count); i++)
            {
                Tracks tracksPagination = sortedAlbums[i].tracks;
                foreach (SimplifiedTrack track in tracksPagination.items)
                {
                    sample.Add(track);
                    await _hubContext.Clients.Client(connectionId)
                            .SendAsync("updateSongsFound", sample.Count);
                }
            }
        }
    }


}
