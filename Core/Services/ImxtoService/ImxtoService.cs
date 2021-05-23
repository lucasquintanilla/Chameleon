using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.ImxtoService
{
    public class ImxtoService
    {
        private HttpClient _client = new HttpClient();

        public async Task<ImxtoFileUploaded> Upload(Stream stream)
        {
            string baseUrl = "https://imx.to/dropzone.php?session_id=68giijgrtpqe0nk9ttlot9noi4";

            MultipartFormDataContent form = new MultipartFormDataContent();
            
            form.Add(new StringContent("3"), "thumbnail_format");
            form.Add(new StringContent("4"), "thumb_size_contaner");
            form.Add(new StringContent("1"), "adult");
            form.Add(new StringContent("Upload"), "simple_upload");

            HttpContent content = new StreamContent(stream);
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "uploaded",
                FileName = "image.jpg"
            };
            form.Add(content);

            HttpResponseMessage response = await _client.PostAsync(baseUrl, form);

            var responseContent = await response.Content.ReadAsStringAsync();

            var document = new HtmlDocument();
            document.LoadHtml(responseContent);

            // https://imx.to/u/t/2021/05/23/2ie1pv.jpg
            var thumbnailUrl = document.DocumentNode.SelectSingleNode("//img").Attributes["src"].Value;

            // https://i.imx.to/i/2021/05/23/2ie1pv.jpg
            var originalUrl = "https://i.imx.to/i" + thumbnailUrl.Substring(18);

            return new ImxtoFileUploaded()
            {
                ThumbnailUrl = thumbnailUrl,
                OriginalUrl = originalUrl
            };
        }
    }

    public class ImxtoFileUploaded
    {
        public string ThumbnailUrl { get; set; }
        public string OriginalUrl { get; set; }
    }
}
