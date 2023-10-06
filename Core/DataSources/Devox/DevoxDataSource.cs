using Core.DataSources.Devox.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Core.DataSources.Devox;

public interface IDevoxDataSource
{
    Task GetVox();
    Task<IEnumerable<Vox>> GetVoxes();
    Task<IEnumerable<Vox>> GetVoxesTest();
    Task<IEnumerable<Vox>> GetMoreVoxes(int count);
}

public class DevoxDataSource : IDevoxDataSource
{
    private readonly HttpClient httpClient = new HttpClient();

    private string Payload = @"{
    ""user"": {
        ""userData"": {
            ""_id"": ""64f514313a07a5f69779e0f5"",
            ""userId"": ""9953c1de-1d63"",
            ""userPassword"": ""$2a$04$h0J5CgP8YCfldq1gDDe1JexBfbIio.CR8hLsPQoCWOlDZnglLhQEC"",
            ""rank"": ""anon"",
            ""hiddenWords"": [],
            ""hiddenCategories"": [
                31,
                10,
                21,
                22,
                15,
                36,
                24,
                23,
                42,
                41
            ],
            ""hiddenVoxs"": [],
            ""notifications"": [],
            ""notificationsSize"": 0
        },
        ""password"": ""391f2510-c1bc"",
        ""condition"": ""success""
    },
    ""count"": 20,
    ""oldCount"": 0
}";
    public async Task<IEnumerable<Vox>> GetVoxes()
    {
        using (var request = new HttpRequestMessage(new HttpMethod("POST"), Constants.GetVoxesEnpoint))
        {
            request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:108.0) Gecko/20100101 Firefox/108.0");
            request.Headers.TryAddWithoutValidation("Accept", "application/json, text/plain, */*");
            request.Headers.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.5");
            //request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
            request.Headers.TryAddWithoutValidation("Origin", "https://devox.me");
            request.Headers.TryAddWithoutValidation("DNT", "1");
            request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
            request.Headers.TryAddWithoutValidation("Referer", "https://devox.me/");
            //request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
            //request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
            //request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "same-site");
            request.Headers.TryAddWithoutValidation("TE", "trailers");

            //request.Content = new StringContent("{\"user\":null,\"count\":36,\"oldCount\":0}");
            request.Content = new StringContent(Payload);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var response = await httpClient.SendAsync(request);

            Console.WriteLine($"{Constants.GetVoxesEnpoint} Response status code: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<Vox>();
            
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);

            var respon = JsonConvert.DeserializeObject<GetVoxesResponse>(content);
            return respon.Voxes;
        }
    }

    public async Task<IEnumerable<Vox>> GetMoreVoxes(int count)
    {
        using (var request = new HttpRequestMessage(new HttpMethod("POST"), Constants.GetVoxesEnpoint))
        {
            request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:108.0) Gecko/20100101 Firefox/108.0");
            request.Headers.TryAddWithoutValidation("Accept", "application/json, text/plain, */*");
            request.Headers.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.5");
            //request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
            request.Headers.TryAddWithoutValidation("Origin", "https://devox.me");
            request.Headers.TryAddWithoutValidation("DNT", "1");
            request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
            request.Headers.TryAddWithoutValidation("Referer", "https://devox.me/");
            //request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
            //request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
            //request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "same-site");
            request.Headers.TryAddWithoutValidation("TE", "trailers");
            var req = new { count = count, oldCount = count - 20 };
            //request.Content = new StringContent("{\"user\":null,\"count\":20,\"oldCount\":20}");
            request.Content = new StringContent(JsonConvert.SerializeObject(req));
            //request.Content = new StringContent(Payload);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var response = await httpClient.SendAsync(request);

            Console.WriteLine($"{Constants.GetVoxesEnpoint} Response status code: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<Vox>();

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);

            var respon = JsonConvert.DeserializeObject<GetVoxesResponse>(content);
            return respon.Voxes;
        }
    }

    public async Task GetVox()
    {
        //https://api.devox.uno/getVox/c6866acd-8514
        //[{ "poll":[false],"_id":"634f9518129544ad402e3bd3","title":"Nunca se habló del cepillismo de Gandhi","description":"Eso.<br>Decía practicar el celibato, pero \"dormía\" en bolas con niñas","category":39,"filename":"c6866acd-8514","fileExtension":"image","isURL":true,"dice":false,"username":"15b3ee24-f0bd-430a-8429-3b763ac48c77","url":"https://upload.wikimedia.org/wikipedia/commons/thumb/2/24/Gandhi_and_Indira_1924.jpg/2560px-Gandhi_and_Indira_1924.jpg","sticky":false,"flag":false,"commentsCount":19,"date":"2022-10-19T06:11:36.980Z","lastUpdate":"2022-11-25T00:26:13.061Z"}]
    }

    public async Task<IEnumerable<Vox>> GetVoxesTest()
    {
        var options = new RestClientOptions("https://go.devox.me")
        {
            MaxTimeout = -1,
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36",
        };
        var client = new RestClient(options);
        var request = new RestRequest("/getVoxes", Method.Post);
        request.AddHeader("authority", "go.devox.me");
        request.AddHeader("accept", "*/*");
        request.AddHeader("accept-language", "en-GB,en-US;q=0.9,en;q=0.8,es;q=0.7");
        request.AddHeader("access-control-request-headers", "content-type");
        request.AddHeader("access-control-request-method", "POST");
        request.AddHeader("origin", "https://devox.me");
        request.AddHeader("referer", "https://devox.me/");
        request.AddHeader("sec-fetch-dest", "empty");
        request.AddHeader("sec-fetch-site", "same-site");
        request.AddHeader("Content-Type", "application/json");
        var body = @"{
" + "\n" +
        @"    ""user"": {
" + "\n" +
        @"        ""userData"": {
" + "\n" +
        @"            ""_id"": ""64f514313a07a5f69779e0f5"",
" + "\n" +
        @"            ""userId"": ""9953c1de-1d63"",
" + "\n" +
        @"            ""userPassword"": ""$2a$04$h0J5CgP8YCfldq1gDDe1JexBfbIio.CR8hLsPQoCWOlDZnglLhQEC"",
" + "\n" +
        @"            ""rank"": ""anon"",
" + "\n" +
        @"            ""hiddenWords"": [],
" + "\n" +
        @"            ""hiddenCategories"": [
" + "\n" +
        @"                31,
" + "\n" +
        @"                10,
" + "\n" +
        @"                21,
" + "\n" +
        @"                22,
" + "\n" +
        @"                15,
" + "\n" +
        @"                36,
" + "\n" +
        @"                24,
" + "\n" +
        @"                23,
" + "\n" +
        @"                42,
" + "\n" +
        @"                41
" + "\n" +
        @"            ],
" + "\n" +
        @"            ""hiddenVoxs"": [],
" + "\n" +
        @"            ""notifications"": [],
" + "\n" +
        @"            ""notificationsSize"": 0
" + "\n" +
        @"        },
" + "\n" +
        @"        ""password"": ""391f2510-c1bc"",
" + "\n" +
        @"        ""condition"": ""success""
" + "\n" +
        @"    },
" + "\n" +
        @"    ""count"": 20,
" + "\n" +
        @"    ""oldCount"": 0
" + "\n" +
        @"}";
        request.AddStringBody(body, DataFormat.Json);
        RestResponse response = await client.ExecuteAsync(request);
        Console.WriteLine(response.Content);

        var respon = JsonConvert.DeserializeObject<GetVoxesResponse>(response.Content);
        return respon.Voxes;
    }
}
