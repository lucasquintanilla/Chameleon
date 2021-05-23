﻿using HtmlAgilityPack;
using Microsoft.Security.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UnitTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Start");

            var service = new ImxToService();
            await service.Upload();

            Console.WriteLine("End");
        }
    }

    public class ImxToService
    {
        private HttpClient _client = new HttpClient();

        public async Task Upload()
        {
            string baseUrl = "https://imx.to/dropzone.php?session_id=68giijgrtpqe0nk9ttlot9noi4";

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("thumbnail_format", "3");
            parameters.Add("thumb_size_contaner", "4");
            parameters.Add("adult", "1");
            parameters.Add("simple_upload", "Upload");

            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("http://localhost:54169");
            MultipartFormDataContent form = new MultipartFormDataContent();
            HttpContent content = new StringContent("fileToUpload");
            //HttpContent DictionaryItems = new FormUrlEncodedContent(parameters);
            //form.Add(content, "fileToUpload");
            form.Add(new StringContent("3"), "thumbnail_format");
            form.Add(new StringContent("4"), "thumb_size_contaner");
            form.Add(new StringContent("1"), "adult");
            form.Add(new StringContent("Upload"), "simple_upload");
            //form.Add(DictionaryItems, "medicineOrder");

            var stream = new FileStream("image.jpg", FileMode.Open);
            content = new StreamContent(stream);
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
        }
    }
}
