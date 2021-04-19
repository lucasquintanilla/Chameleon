using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Voxed.WebApp.Services
{
    public class YoutubeService
    {
        //private readonly HttpClient client = new HttpClient();

        //public async Task<string> GenerateThumbnail(string id, string hash)
        //{
        //    var response = await client.GetAsync($"https://img.youtube.com/vi/{id}/maxresdefault.jpg");

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        response = await client.GetAsync($"https://img.youtube.com/vi/{id}/hqdefault.jpg");
        //    }

        //    if (!response.IsSuccessStatusCode) return null;

        //    using var stream = await response.Content.ReadAsStreamAsync();

        //    var thumbnailFilename = $"{DateTime.Now:yyyyMMdd}-{hash}.jpg";
        //    var thumbnailFilePath = Path.Combine(_dir, folderName, thumbnailFilename);

        //    using var fileStream = File.Create($"{CarpetaDeAlmacenamiento}/{media.VistaPreviaLocal}");
        //    stream.Seek(0, SeekOrigin.Begin);
        //    await stream.CopyToAsync(fileStream);
        //}
    }
}
