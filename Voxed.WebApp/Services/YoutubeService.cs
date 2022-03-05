using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Voxed.WebApp.Services
{
    public class YoutubeService
    {
        private readonly HttpClient _client;

        public YoutubeService()
        {
            _client = new HttpClient();
        }

        public async Task<Stream> GetYoutubeThumbnailStream(string videoId)
        {
            var response = await _client.GetAsync($"https://img.youtube.com/vi/{videoId}/maxresdefault.jpg");

            if (!response.IsSuccessStatusCode)
            {
                response = await _client.GetAsync($"https://img.youtube.com/vi/{videoId}/hqdefault.jpg");
            }

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadAsStreamAsync();
        }
    }
}
