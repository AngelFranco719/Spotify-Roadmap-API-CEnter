using Microsoft.Extensions.Logging;
using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Models.Entities;
using SpotifyRequestManagement.Models.Multiple_Entities;
using SpotifyRequestManagement.Models.Pages;
using SpotifyRequestManagement.Models.Simplified_Entities;
using System.Collections.Concurrent;

namespace SpotifyRequestManagement.Services
{
    public class GetTracksFromArtists
    {
        public List<Artist> flattenedGraph { get; set; }
        SpotifyApiRequest spotifyApiRequest;
        List<Task> tasks = new List<Task>();
        ILogger<GetTracksFromArtists> logger;
        ConcurrentBag<SimplifiedTrack> sample = new ConcurrentBag<SimplifiedTrack>(); 

        public GetTracksFromArtists(
                                    SpotifyApiRequest spotifyApiRequest, 
                                    ILogger<GetTracksFromArtists> logger)
        {
            this.flattenedGraph = flattenedGraph;
            this.spotifyApiRequest = spotifyApiRequest;
            this.logger = logger;
        }

        public async Task<ConcurrentBag<SimplifiedTrack>> getTracks() {
            int maxConcurrentRequest = 2;
            int delay = 1000; 
            SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrentRequest);

            foreach (Artist artist in flattenedGraph) {
                await semaphore.WaitAsync();
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await this.GetSimplifiedAlbumsFromArtist(artist.id);
                    }
                    catch(Exception e)
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

        #region Obtener Albumes de un Artista
        private async Task GetSimplifiedAlbumsFromArtist(string artistID)
        {
            AlbumPages albumPages = await spotifyApiRequest.getArtistAlbums(artistID);
            await GetSpecifiedAlbumsFromArtist(albumPages);
        }

        private async Task GetSpecifiedAlbumsFromArtist(AlbumPages albumPages)
        {
            List<string> grouped_albums = new List<string>();
            foreach (SimplifiedAlbum simplifiedAlbum in albumPages.items) // <- Agrupo los albums.
                grouped_albums.Add(simplifiedAlbum.id);

            Albums result = await spotifyApiRequest.getAlbums(grouped_albums);

            List<Album> sorted = result.albums.OrderByDescending(x => x.popularity).ToList();

            for (int i = 0; i < Math.Min(5, sorted.Count); i++) 
                this.mapSimplifiedTracksInSample(sorted[i]);
            
        }

        private void mapSimplifiedTracksInSample(Album current)
        {
            Tracks tracksPagination = current.tracks;
            SimplifiedTrack[] simplifiedTracks = tracksPagination.items;
            foreach (SimplifiedTrack track in simplifiedTracks)
                sample.Add(track);
        }
        #endregion
    }
}
