using Core.DataSources.Devox.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Core.DataSources.Devox;

public interface IPostSource<T>
{
    Task<T> GetPost(string postId);
    Task<IEnumerable<T>> GetPosts(int? count = 0);
}

public interface IDevoxDataSource : IPostSource<DevoxPost>
{
    Task<DevoxPost> GetPost(string postId);
    Task<IEnumerable<DevoxPost>> GetVoxes();
    Task<IEnumerable<DevoxPost>> GetMoreVoxes(int count);
}

public class DevoxDataSource : IDevoxDataSource
{
    private HttpClient _httpClient;
    private RestClient _restClient;

    public DevoxDataSource()
    {
        var proxy = new WebProxy
        {
            Address = new Uri($"http://181.209.78.76:999"),
            BypassProxyOnLocal = false,
            UseDefaultCredentials = false,

            // Proxy credentials
            //Credentials = new NetworkCredential(
            //userName: "YOUR_API_KEY",
            //password: "render_js=False&premium_proxy=True")
        };

        var httpClientHandler = new HttpClientHandler
        {
            Proxy = proxy,
        };

        // Disable SSL verification
        httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        httpClientHandler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;

        _httpClient = new HttpClient(handler: httpClientHandler, disposeHandler: true);
        _restClient = new RestClient(_httpClient);
    }

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
    public async Task<IEnumerable<DevoxPost>> GetVoxes()
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

            var response = await _httpClient.SendAsync(request);

            Console.WriteLine($"{Constants.GetVoxesEnpoint} Response status code: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<DevoxPost>();
            
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);

            var respon = JsonConvert.DeserializeObject<GetVoxesResponse>(content);
            return respon.Voxes;
        }
    }

    public async Task<IEnumerable<DevoxPost>> GetMoreVoxes(int count)
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

            var response = await _httpClient.SendAsync(request);

            Console.WriteLine($"{Constants.GetVoxesEnpoint} Response status code: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<DevoxPost>();

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);

            var respon = JsonConvert.DeserializeObject<GetVoxesResponse>(content);
            return respon.Voxes;
        }
    }

    public async Task<DevoxPost> GetPost(string postId)
    {
        return null;
        //https://api.devox.uno/getVox/c6866acd-8514
        //[{ "poll":[false],"_id":"634f9518129544ad402e3bd3","title":"Nunca se habló del cepillismo de Gandhi","description":"Eso.<br>Decía practicar el celibato, pero \"dormía\" en bolas con niñas","category":39,"filename":"c6866acd-8514","fileExtension":"image","isURL":true,"dice":false,"username":"15b3ee24-f0bd-430a-8429-3b763ac48c77","url":"https://upload.wikimedia.org/wikipedia/commons/thumb/2/24/Gandhi_and_Indira_1924.jpg/2560px-Gandhi_and_Indira_1924.jpg","sticky":false,"flag":false,"commentsCount":19,"date":"2022-10-19T06:11:36.980Z","lastUpdate":"2022-11-25T00:26:13.061Z"}]
    }

    public async Task<IEnumerable<DevoxPost>> GetPosts(int? count = 20)
    {
        var request = new RestRequest(Constants.GetVoxesEnpoint, Method.Post);
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

        //var req = new { 
        //    count = count, 
        //    oldCount = count - 20, 
        //    user = new { 
        //        userdata = new { 
        //            _id = "64f514313a07a5f69779e0f5", 
        //            userId = "9953c1de-1d63",
        //            userPassword = "$2a$04$h0J5CgP8YCfldq1gDDe1JexBfbIio.CR8hLsPQoCWOlDZnglLhQEC"
        //        }
        //    } 
        //};

        //var req = new
        //{
        //    count = count,
        //    oldCount = count - 20,
        //    macro = "g"
        //};
        //var body = JsonConvert.SerializeObject(req);
        request.AddStringBody(body, DataFormat.Json);
        request.Timeout = 2000;

        RestResponse response = await _restClient.ExecuteAsync(request);
        //while (response.IsSuccessStatusCode is false)
        //{
        //    Thread.Sleep(500);
        //    response = await _restClient.ExecuteAsync(request);
        //}
        Console.WriteLine(response.Content);

        //var content = @"""{\""sticky\"":null,\""voxes\"":[{\""title\"":\""Como viene de colacha mi match?\"",\""category\"":13,\""filename\"":\""5ef664c3-1047\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":32,\""poll\"":null,\""date\"":\""2024-01-28T21:26:49.233Z\"",\""lastUpdate\"":\""2024-01-28T21:33:06.717Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""Estas laburando en coto por un sueldo minimo y ves esto, que procede?\"",\""category\"":5,\""filename\"":\""d38bd066-a1e2\"",\""fileExtension\"":\""mp4\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":25,\""poll\"":null,\""date\"":\""2024-01-28T20:27:42.037Z\"",\""lastUpdate\"":\""2024-01-28T21:33:06.509Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""Están escrachando a belirran en fcbk\"",\""category\"":44,\""filename\"":\""c34c8e06-f26f\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":43,\""poll\"":null,\""date\"":\""2024-01-28T18:10:44.749Z\"",\""lastUpdate\"":\""2024-01-28T21:33:06.391Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""MEXICANA cuenta su EXPERIENCIA ESTUDIANDO en ARGENTINA KJJ\"",\""category\"":13,\""filename\"":\""68998fd8-0a9e\"",\""fileExtension\"":\""mp4\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":66,\""poll\"":null,\""date\"":\""2024-01-28T13:32:40.28Z\"",\""lastUpdate\"":\""2024-01-28T21:33:04.68Z\"",\""pin\"":\""64B4F76\"",\""flag\"":true,\""username\"":null},{\""title\"":\""Por que el ejercito Uruguayo se quedo en los 50s  no siendo un pais comunista? \"",\""category\"":27,\""filename\"":\""42fad634-5f10\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":14,\""poll\"":null,\""date\"":\""2024-01-28T21:01:16.184Z\"",\""lastUpdate\"":\""2024-01-28T21:33:03.274Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""El que me alquiló se debe querer matar kjjjj\"",\""category\"":13,\""filename\"":\""e7b5312e-5b25\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":55,\""poll\"":null,\""date\"":\""2024-01-28T15:36:24.643Z\"",\""lastUpdate\"":\""2024-01-28T21:32:56.975Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""Te amo\"",\""category\"":44,\""filename\"":\""bb161782-724e\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":77,\""poll\"":null,\""date\"":\""2024-01-28T18:23:11.571Z\"",\""lastUpdate\"":\""2024-01-28T21:32:54.776Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""Esta película me dejo al balo\"",\""category\"":13,\""filename\"":\""b464548b-07d7\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":2,\""poll\"":null,\""date\"":\""2024-01-28T21:30:38.656Z\"",\""lastUpdate\"":\""2024-01-28T21:32:48.473Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""Esta bien? Es mala cancion? O ciclo terminado? \"",\""category\"":34,\""filename\"":\""705909fc-c06c\"",\""fileExtension\"":\""video\"",\""isURL\"":true,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""NaoNcERVF78\"",\""isBucket\"":true,\""commentsCount\"":75,\""poll\"":[[{\""Key\"":\""count\"",\""Value\"":13},{\""Key\"":\""text\"",\""Value\"":\""Es mala cancion y por eso, esos numeros.\""},{\""Key\"":\""index\"",\""Value\"":1}],[{\""Key\"":\""count\"",\""Value\"":2},{\""Key\"":\""text\"",\""Value\"":\""Jeje no c\""},{\""Key\"":\""index\"",\""Value\"":4}]],\""date\"":\""2024-01-28T20:53:44.81Z\"",\""lastUpdate\"":\""2024-01-28T21:32:48.436Z\"",\""pin\"":\""58FFF0C\"",\""flag\"":false,\""username\"":null},{\""title\"":\""Los \\u0026quot;busca fama \\u0026quot; de los Andes\"",\""category\"":13,\""filename\"":\""b20cd256-1c62\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":76,\""poll\"":null,\""date\"":\""2024-01-25T00:18:25.248Z\"",\""lastUpdate\"":\""2024-01-28T21:32:47.333Z\"",\""pin\"":\""F434DCC\"",\""flag\"":false,\""username\"":null},{\""title\"":\""¿Qué anda haciendo el anon este 4to domingo del 2024?\"",\""category\"":27,\""filename\"":\""3b976f36-71ae\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":8,\""poll\"":null,\""date\"":\""2024-01-28T21:23:31.225Z\"",\""lastUpdate\"":\""2024-01-28T21:32:42.507Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""Que hace el Aloe Vera en la cara, cuerpo y demás?\"",\""category\"":5,\""filename\"":\""f0479373-2945\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":0,\""poll\"":null,\""date\"":\""2024-01-28T21:32:34.622Z\"",\""lastUpdate\"":\""2024-01-28T21:32:34.622Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""COMO APRENDER JAPONÉS [LA POSTA] [MEGAPOST]\"",\""category\"":1,\""filename\"":\""efbb4301-42e7\"",\""fileExtension\"":\""video\"",\""isURL\"":true,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""t1v_9oTgQ7Q\"",\""isBucket\"":true,\""commentsCount\"":2,\""poll\"":null,\""date\"":\""2024-01-28T21:32:02.988Z\"",\""lastUpdate\"":\""2024-01-28T21:32:30.899Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""EL MESSI DE LA INDIA\"",\""category\"":13,\""filename\"":\""c82b8ef1-c1d3\"",\""fileExtension\"":\""mp4\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":5,\""poll\"":null,\""date\"":\""2024-01-28T20:58:37.106Z\"",\""lastUpdate\"":\""2024-01-28T21:32:30.35Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""ustedes son todo lo que tengo gordos \"",\""category\"":13,\""filename\"":\""02698468-9759\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":16,\""poll\"":null,\""date\"":\""2024-01-28T21:02:21.636Z\"",\""lastUpdate\"":\""2024-01-28T21:32:25.354Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""VIVA ROCA\"",\""category\"":19,\""filename\"":\""95a57d83-7126\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":43,\""poll\"":null,\""date\"":\""2023-12-27T21:32:14.031Z\"",\""lastUpdate\"":\""2024-01-28T21:32:23.317Z\"",\""pin\"":null,\""flag\"":true,\""username\"":null},{\""title\"":\""Le saqué una foto a mi empleada doméstica cuando fuimos a comprar al almacén de pablito\"",\""category\"":6,\""filename\"":\""a04231e1-0a2b\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":0,\""poll\"":null,\""date\"":\""2024-01-28T21:32:20.951Z\"",\""lastUpdate\"":\""2024-01-28T21:32:20.951Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""Mi sueño es engordar a Liu\"",\""category\"":17,\""filename\"":\""b0059015-4b04\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":8,\""poll\"":[[{\""Key\"":\""count\"",\""Value\"":1},{\""Key\"":\""text\"",\""Value\"":\""Buen plan op\""},{\""Key\"":\""index\"",\""Value\"":1}],[{\""Key\"":\""count\"",\""Value\"":0},{\""Key\"":\""text\"",\""Value\"":\""Descartadisima lechona\""},{\""Key\"":\""index\"",\""Value\"":2}]],\""date\"":\""2024-01-28T21:26:11.795Z\"",\""lastUpdate\"":\""2024-01-28T21:32:10.492Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""CONFIRMADO le pidieron a Moreno que no hable más de negros en los reportajes\"",\""category\"":26,\""filename\"":\""8c576677-dc0b\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":5,\""poll\"":null,\""date\"":\""2024-01-28T17:33:11.773Z\"",\""lastUpdate\"":\""2024-01-28T21:32:08.002Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""Me siento mejor solo que cuando estoy con gente\"",\""category\"":13,\""filename\"":\""cf513cc2-92fc\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":3,\""poll\"":null,\""date\"":\""2024-01-28T21:21:52.722Z\"",\""lastUpdate\"":\""2024-01-28T21:31:55.803Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""YO TAMBIÉN AMO A CAMILA DALTO Y VOS GORDO\"",\""category\"":13,\""filename\"":\""14be863a-f350\"",\""fileExtension\"":\""gif\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":25,\""poll\"":null,\""date\"":\""2024-01-28T14:49:41.586Z\"",\""lastUpdate\"":\""2024-01-28T21:31:52.026Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""¿Puedo remontar mi vida con 23 años?\"",\""category\"":5,\""filename\"":\""2e6a67ba-8a0a\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":47,\""poll\"":null,\""date\"":\""2024-01-28T14:18:46.383Z\"",\""lastUpdate\"":\""2024-01-28T21:31:49.538Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""Que opinan de este tipo anons \"",\""category\"":13,\""filename\"":\""4971b328-fed0\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":3,\""poll\"":null,\""date\"":\""2024-01-28T21:29:03.911Z\"",\""lastUpdate\"":\""2024-01-28T21:31:48.258Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""¿Del uno al diez, como se califica el anon en términos de facha?\"",\""category\"":27,\""filename\"":\""e50e7e33-9fc1\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":0,\""poll\"":null,\""date\"":\""2024-01-28T21:31:39.744Z\"",\""lastUpdate\"":\""2024-01-28T21:31:39.744Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""July3p pioló a una chica en La Plata hace mucho\"",\""category\"":17,\""filename\"":\""3d140f74-4412\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":14,\""poll\"":null,\""date\"":\""2024-01-28T20:06:10.27Z\"",\""lastUpdate\"":\""2024-01-28T21:31:37.997Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""¿Por qué Liliana limones no reconoce su multicuenta?\"",\""category\"":20,\""filename\"":\""407e992b-c2d2\"",\""fileExtension\"":\""mp4\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":18,\""poll\"":null,\""date\"":\""2024-01-28T21:10:57.113Z\"",\""lastUpdate\"":\""2024-01-28T21:31:37.882Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""Saga faneada por bolivianos por excelencia\"",\""category\"":14,\""filename\"":\""640d310c-c6e0\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":6,\""poll\"":null,\""date\"":\""2024-01-28T20:02:40.38Z\"",\""lastUpdate\"":\""2024-01-28T21:31:35.4Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""otro dia sin un candado en la reja\"",\""category\"":44,\""filename\"":\""fedf9a53-3713\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":19,\""poll\"":null,\""date\"":\""2024-01-26T22:37:41.493Z\"",\""lastUpdate\"":\""2024-01-28T21:31:34.112Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""[TIERLIST INDUSCTIBLE DEL OMNIVERSO]\"",\""category\"":44,\""filename\"":\""1ca9d931-e863\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":116,\""poll\"":null,\""date\"":\""2024-01-28T03:10:03.54Z\"",\""lastUpdate\"":\""2024-01-28T21:31:27.519Z\"",\""pin\"":\""98B57C7\"",\""flag\"":false,\""username\"":null},{\""title\"":\""Aca jugando a la plei dos\"",\""category\"":13,\""filename\"":\""3b2236c0-5080\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":22,\""poll\"":null,\""date\"":\""2024-01-28T21:19:59.831Z\"",\""lastUpdate\"":\""2024-01-28T21:31:27.084Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""¿Porque carajo no hacen nada al gordo Belizan?\"",\""category\"":27,\""filename\"":\""ef469095-10cf\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":2,\""poll\"":null,\""date\"":\""2024-01-28T21:30:38.297Z\"",\""lastUpdate\"":\""2024-01-28T21:31:18.63Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""Alguien doxee al gordo Fabian.\"",\""category\"":13,\""filename\"":\""6f3f2774-53eb\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":0,\""poll\"":null,\""date\"":\""2024-01-28T21:31:11.919Z\"",\""lastUpdate\"":\""2024-01-28T21:31:11.919Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""Cuando voy en metro siento que todos me quedan viendo y me juzgan\"",\""category\"":13,\""filename\"":\""630c8d92-1741\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":26,\""poll\"":null,\""date\"":\""2024-01-28T18:00:59.086Z\"",\""lastUpdate\"":\""2024-01-28T21:31:10.069Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""¿Garba ser sicario?\"",\""category\"":27,\""filename\"":\""d335bb73-a37f\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":3,\""poll\"":null,\""date\"":\""2024-01-28T21:10:00.477Z\"",\""lastUpdate\"":\""2024-01-28T21:31:03.113Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""El LoL tiene los dias contados\"",\""category\"":14,\""filename\"":\""b4ca57dd-c87e\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":20,\""poll\"":null,\""date\"":\""2024-01-28T20:07:14.53Z\"",\""lastUpdate\"":\""2024-01-28T21:30:51.403Z\"",\""pin\"":null,\""flag\"":true,\""username\"":null},{\""title\"":\""nunca tuve esta sensacion en mi vida\"",\""category\"":17,\""filename\"":\""d434a05e-51e6\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":30,\""poll\"":null,\""date\"":\""2024-01-28T20:08:59.992Z\"",\""lastUpdate\"":\""2024-01-28T21:30:49.894Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""Asi termina el sueño putaku: separado, viviendo en el medio de la nada en un pais que te odian.\"",\""category\"":32,\""filename\"":\""762f08c1-e7b6\"",\""fileExtension\"":\""mp4\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":272,\""poll\"":null,\""date\"":\""2024-01-27T00:43:08.382Z\"",\""lastUpdate\"":\""2024-01-28T21:30:16.673Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""por que mierda los francotiradores apuntan a la cabeza y no al torso?\"",\""category\"":27,\""filename\"":\""8d2b4326-b112\"",\""fileExtension\"":\""jpeg\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":9,\""poll\"":null,\""date\"":\""2024-01-28T20:54:31.345Z\"",\""lastUpdate\"":\""2024-01-28T21:30:09.156Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""AHHHHHHHHH AHHHHHHHHHH AHHHHHHH\"",\""category\"":13,\""filename\"":\""e654d99f-66fd\"",\""fileExtension\"":\""mp4\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":4,\""poll\"":null,\""date\"":\""2024-01-28T21:26:55.688Z\"",\""lastUpdate\"":\""2024-01-28T21:30:05.338Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null},{\""title\"":\""voxero gozador suelto \"",\""category\"":20,\""filename\"":\""c3f0240a-2616\"",\""fileExtension\"":\""mp4\"",\""isURL\"":false,\""dice\"":false,\""blur\"":false,\""sticky\"":false,\""url\"":\""\"",\""isBucket\"":true,\""commentsCount\"":2,\""poll\"":null,\""date\"":\""2024-01-28T21:28:53.548Z\"",\""lastUpdate\"":\""2024-01-28T21:29:54.264Z\"",\""pin\"":null,\""flag\"":false,\""username\"":null}]}""";
        //var content = @"{""sticky"":null,""voxes"":[{""title"":""MEXICANA cuenta su EXPERIENCIA ESTUDIANDO en ARGENTINA KJJ"",""category"":13,""filename"":""68998fd8-0a9e"",""fileExtension"":""mp4"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":40,""poll"":null,""date"":""2024-01-28T13:32:40.28Z"",""lastUpdate"":""2024-01-28T21:13:17.1Z"",""pin"":""64B4F76"",""flag"":true,""username"":null},{""title"":""si tenés más de 30 y no haceés nada...No sos nini..sos un Mirrey"",""category"":13,""filename"":""7b79aca6-cdf8"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":12,""poll"":null,""date"":""2024-01-28T20:14:47.928Z"",""lastUpdate"":""2024-01-28T21:13:12.168Z"",""pin"":""8DE31E6"",""flag"":false,""username"":null},{""title"":""casi todos los vox de la home son mios "",""category"":13,""filename"":""2116cbda-32af"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":19,""poll"":null,""date"":""2024-01-28T20:33:18.326Z"",""lastUpdate"":""2024-01-28T21:13:11.106Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""LOS CHILENOS MÁS ARIOS"",""category"":13,""filename"":""095aaa84-2b88"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":16,""poll"":null,""date"":""2024-01-27T00:01:47.892Z"",""lastUpdate"":""2024-01-28T21:13:04.929Z"",""pin"":null,""flag"":false,""username"":null},{""title"":"" "",""category"":13,""filename"":""c848322e-4b72"",""fileExtension"":""mp4"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":6,""poll"":null,""date"":""2024-01-28T20:51:54.41Z"",""lastUpdate"":""2024-01-28T21:13:04.559Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""¿vieron eso?"",""category"":17,""filename"":""f1fdda60-3a40"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":8,""poll"":null,""date"":""2024-01-28T21:08:08.684Z"",""lastUpdate"":""2024-01-28T21:12:59.581Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""YO TAMBIÉN AMO A CAMILA DALTO Y VOS GORDO"",""category"":13,""filename"":""14be863a-f350"",""fileExtension"":""gif"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":18,""poll"":null,""date"":""2024-01-28T14:49:41.586Z"",""lastUpdate"":""2024-01-28T21:12:49.577Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Tengo una amiga venezolana q se vive quejando del pais"",""category"":13,""filename"":""150ce064-d300"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":false,""commentsCount"":23,""poll"":[false],""date"":""2023-10-24T21:27:35.95Z"",""lastUpdate"":""2024-01-28T21:12:45.585Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""devoxera, te redpilleo"",""category"":13,""filename"":""e6a6c26e-70bd"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":1,""poll"":null,""date"":""2024-01-28T21:11:03.652Z"",""lastUpdate"":""2024-01-28T21:12:40.514Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""tengo una pc gamer pero juego cosas viejas"",""category"":17,""filename"":""8e05166e-e05e"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":5,""poll"":null,""date"":""2024-01-28T21:06:14.643Z"",""lastUpdate"":""2024-01-28T21:12:38.774Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""¿Cuán pobre tipo sos si no podés comprarte este perfume?"",""category"":5,""filename"":""83e9cd27-0616"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":50,""poll"":null,""date"":""2024-01-28T20:29:27.066Z"",""lastUpdate"":""2024-01-28T21:12:26.869Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Che gordo motero"",""category"":27,""filename"":""cf44c2fc-2103"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":8,""poll"":null,""date"":""2024-01-28T20:58:06.228Z"",""lastUpdate"":""2024-01-28T21:12:05.124Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""[FIN] SERIO HAN DOMADO NO PUEDO TRABAJAR NI DE RAPPI (NO BAIT)"",""category"":13,""filename"":""03bae915-8a48"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":49,""poll"":null,""date"":""2024-01-28T20:30:17.563Z"",""lastUpdate"":""2024-01-28T21:12:04.786Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Hay algo más allá de la Antártida? "",""category"":27,""filename"":""bdf026f7-5553"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":68,""poll"":null,""date"":""2024-01-27T22:44:48.625Z"",""lastUpdate"":""2024-01-28T21:11:10.729Z"",""pin"":""486B051"",""flag"":false,""username"":null},{""title"":""Los \u0026quot;busca fama \u0026quot; de los Andes"",""category"":13,""filename"":""b20cd256-1c62"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":65,""poll"":null,""date"":""2024-01-25T00:18:25.248Z"",""lastUpdate"":""2024-01-28T21:11:00.776Z"",""pin"":""F434DCC"",""flag"":false,""username"":null},{""title"":""Si quieren los IG de Sofia, aca estan."",""category"":13,""filename"":""829a2843-e407"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":2,""poll"":null,""date"":""2024-01-28T21:06:58.734Z"",""lastUpdate"":""2024-01-28T21:10:58.114Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""¿Garba ser sicario?"",""category"":27,""filename"":""d335bb73-a37f"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":0,""poll"":null,""date"":""2024-01-28T21:10:00.477Z"",""lastUpdate"":""2024-01-28T21:10:00.477Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""como alguien puede ser asi de Gordo? "",""category"":27,""filename"":""1886413e-2f95"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":10,""poll"":null,""date"":""2024-01-28T20:49:54.001Z"",""lastUpdate"":""2024-01-28T21:09:32.239Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Como puedo tramitar algun certificado de discapacidad para no trabajar nunca mas?"",""category"":27,""filename"":""83e8c646-2a46"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":3,""poll"":null,""date"":""2024-01-28T21:03:15.271Z"",""lastUpdate"":""2024-01-28T21:09:22.834Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Hermanos uruguayos gracias por inventar el Mate 🧉"",""category"":13,""filename"":""7d6976c4-7744"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":15,""poll"":null,""date"":""2024-01-28T13:01:05.005Z"",""lastUpdate"":""2024-01-28T21:09:18.794Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""¿Cine porno en Buenos Aires?"",""category"":27,""filename"":""0f396c4d-a9ab"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":27,""poll"":null,""date"":""2024-01-28T18:38:49.393Z"",""lastUpdate"":""2024-01-28T21:06:37.564Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Dudo mucho que exista alguien mejor que yo en el bt3 en esta pagina."",""category"":13,""filename"":""d966dcd7-42fc"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":42,""poll"":null,""date"":""2024-01-28T17:15:09.02Z"",""lastUpdate"":""2024-01-28T21:06:13.129Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Querés saber si sos manicero? Entrá gordo que te tiro la posta"",""category"":5,""filename"":""3be447eb-6e79"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":0,""poll"":null,""date"":""2024-01-28T21:05:45.399Z"",""lastUpdate"":""2024-01-28T21:05:45.399Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Me siento autisticamente  identificado con la sidosa"",""category"":13,""filename"":""bcbfab81-515b"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":7,""poll"":null,""date"":""2024-01-28T20:55:07.736Z"",""lastUpdate"":""2024-01-28T21:05:24.01Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""EL MESSI DE LA INDIA"",""category"":13,""filename"":""c82b8ef1-c1d3"",""fileExtension"":""mp4"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":2,""poll"":null,""date"":""2024-01-28T20:58:37.106Z"",""lastUpdate"":""2024-01-28T21:05:10.055Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""ustedes son todo lo que tengo gordos "",""category"":13,""filename"":""02698468-9759"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":7,""poll"":null,""date"":""2024-01-28T21:02:21.636Z"",""lastUpdate"":""2024-01-28T21:05:07.889Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Como rompen las bolas con los downs los mogolicos estos "",""category"":13,""filename"":""2013b9aa-6526"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":1,""poll"":null,""date"":""2024-01-28T21:04:34.083Z"",""lastUpdate"":""2024-01-28T21:05:04.237Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Si fuera down me dedicaria a robar y violar impunemente "",""category"":13,""filename"":""a4ece970-4f93"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":5,""poll"":null,""date"":""2024-01-28T20:21:37.817Z"",""lastUpdate"":""2024-01-28T21:04:52.042Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Garpa bailar como liby_sex?"",""category"":27,""filename"":""5e855c10-fb3e"",""fileExtension"":""mp4"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":39,""poll"":null,""date"":""2024-01-28T18:49:58.558Z"",""lastUpdate"":""2024-01-28T21:04:28.747Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Los machos tenkaichi 3 solo jugamos usando combos"",""category"":5,""filename"":""c63fb700-ad26"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":2,""poll"":null,""date"":""2024-01-28T21:03:19.842Z"",""lastUpdate"":""2024-01-28T21:04:26.155Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Por que el ejercito Uruguayo se quedo en los 50s  no siendo un pais comunista? "",""category"":27,""filename"":""42fad634-5f10"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":9,""poll"":null,""date"":""2024-01-28T21:01:16.184Z"",""lastUpdate"":""2024-01-28T21:04:19.898Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Tengo piojos o pulgas en el pito"",""category"":17,""filename"":""9b5af2a1-6a30"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":10,""poll"":null,""date"":""2023-12-15T21:33:01.989Z"",""lastUpdate"":""2024-01-28T21:03:10.041Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""No tienes enemigos. Nadie es tu enemigo, anon."",""category"":5,""filename"":""7bbefc3b-d4c4"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":5,""poll"":null,""date"":""2024-01-28T20:59:37.673Z"",""lastUpdate"":""2024-01-28T21:03:01.106Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""¿Puedo remontar mi vida con 23 años?"",""category"":5,""filename"":""2e6a67ba-8a0a"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":45,""poll"":null,""date"":""2024-01-28T14:18:46.383Z"",""lastUpdate"":""2024-01-28T21:01:01.489Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Me aburri de salir con comunachas"",""category"":13,""filename"":""fceb7adc-48bc"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":1,""poll"":null,""date"":""2024-01-28T20:46:16.718Z"",""lastUpdate"":""2024-01-28T21:00:11.481Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Una mujer me grita barbudo cuando paso por su casa"",""category"":17,""filename"":""d7c1f28c-2c5a"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":4,""poll"":null,""date"":""2024-01-28T20:29:47.096Z"",""lastUpdate"":""2024-01-28T21:00:06.736Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Debe ser difícil ser una de estas [redactazo]"",""category"":13,""filename"":""136933be-2bb9"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":73,""poll"":null,""date"":""2024-01-28T19:55:50.889Z"",""lastUpdate"":""2024-01-28T20:59:54.722Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Asi vemos a los ARJENSIMIOS en el primer mundo"",""category"":13,""filename"":""8898db76-5df6"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":5,""poll"":null,""date"":""2024-01-28T20:17:55.424Z"",""lastUpdate"":""2024-01-28T20:59:54.175Z"",""pin"":null,""flag"":true,""username"":null},{""title"":""Voy a morir virgen porque me da asco el contacto físico"",""category"":13,""filename"":""47e48751-26a7"",""fileExtension"":""jpeg"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":8,""poll"":null,""date"":""2024-01-28T20:48:16.378Z"",""lastUpdate"":""2024-01-28T20:59:39.529Z"",""pin"":null,""flag"":false,""username"":null},{""title"":""Garba ir a toquetear tetubis a corea del sur?"",""category"":27,""filename"":""ef4c0f5d-4536"",""fileExtension"":""mp4"",""isURL"":false,""dice"":false,""blur"":false,""sticky"":false,""url"":"""",""isBucket"":true,""commentsCount"":4,""poll"":null,""date"":""2024-01-28T20:52:39.745Z"",""lastUpdate"":""2024-01-28T20:59:24.13Z"",""pin"":null,""flag"":false,""username"":null}]}";
        var respon = JsonConvert.DeserializeObject<GetVoxesResponse>(response.Content);
        //var respon = JsonConvert.DeserializeObject<GetVoxesResponse>(content);
        return respon.Voxes;
    }
}
