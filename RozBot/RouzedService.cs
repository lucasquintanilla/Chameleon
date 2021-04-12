using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RozBot
{
    public class RouzedService
    {
        private readonly HttpClient client;

        public RouzedService()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            handler.UseCookies = true; //<-- Enable the use of cookies.

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://www.rouzed.club/");
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36");
            client.DefaultRequestHeaders.Add("RequestVerificationToken", "CfDJ8ITZg0LCa_lCmORTOxIT0AAJwTtZAdiDmDGuWwDiOh4qolxTRPkASh22rqRZkYm0vh-V0VEZJIXe_XsBG5GjuaPE1_SLi2u7hHXYUYwUEJkTFne59_1UajEvqjjrhRfOAaR0byV0DvoAL6Pa9bC0inQ");

        }

        public async Task Start()
        {
            //var success = await EnterToWebPage();

            //if (success)
            //{
            //    await LoginWithTokenAsync();
            await LoginAsync();

            //    if (success)
            //        response = await client.PostAsync(string.Empty, requestContent);

            //}
        }

        public async Task<bool> EnterToWebPage()
        {
            var response = await client.GetAsync("login");

            //_antiForgeryToken = await EnsureAntiforgeryToken();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //string responseBody = "";
                //using (HttpContent content = response.Content)
                //{
                //    // ... Read the string.
                //    Task<string> result = content.ReadAsStringAsync();
                //    responseBody = result.Result;
                //}
                //// Below through the regular expression found hidden domain name = '__ RequestVerificationToken'
                //string patternRegion = "<\\s*input\\s*.*name\\s*=\\s*\"__RequestVerificationToken\"\\s*.*value\\s*=\\s*\"(?<value>[\\w-]{108,108})\"\\s*/>";
                //RegexOptions regexOptions = RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled;
                //Regex reg = new Regex(patternRegion, regexOptions);
                //MatchCollection mc = reg.Matches(responseBody);
                //foreach (Match m in mc)
                //{
                //    var hidRequestVerificationToken = m.Groups["value"].Value;
                //    //showlables.Content = hidRequestVerificationToken;
                //}


                return true;
            }

            return false;
        }

        private async Task<bool> LoginWithTokenAsync()
        {
            var loginRequest = new LoginTokenRequestModel() { Token = "FBE7TYFA9T51MZYYMHA7M1EPQ9EL8ZOM72IZHZKG" };
            var requestContent = ConvertToStringContent(loginRequest);


            var response = await client.PostAsync("api/Usuario/RestaurarSesion", requestContent);

            if (!response.IsSuccessStatusCode)
                return false;

            var responseContent = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseContent))
                return false;

            //var token = JsonConvert.DeserializeObject<LoginResponseModel>(responseContent).AccessToken;

            //if (string.IsNullOrEmpty(token))
            //    return false;

            //SetToken(token);
            return true;
        }

        private async Task<bool> LoginAsync()
        {
            var loginRequest = new LoginRequestModel() { Nick = "pepitodefiesta", Contraseña = "pepitodefiesta" };
            var requestContent = ConvertToStringContent(loginRequest);

            var response = await client.PostAsync("api/Usuario/Login", requestContent);

            if (!response.IsSuccessStatusCode)
                return false;

            var responseContent = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseContent))
                return false;

            //var token = JsonConvert.DeserializeObject<LoginResponseModel>(responseContent).AccessToken;

            //if (string.IsNullOrEmpty(token))
            //    return false;

            //SetToken(token);
            return true;
        }

        private StringContent ConvertToStringContent(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }

        protected async Task<AntiForgeryToken> EnsureAntiforgeryToken()
        {
            var antiForgerytoken = new AntiForgeryToken();

            var response = await client.GetAsync("/api/AntiForgery/antiforgery");
            response.EnsureSuccessStatusCode();

            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            {
                var cookies = SetCookieHeaderValue.ParseList(values.ToList());

                //var _antiforgeryCookie = cookies.SingleOrDefault(c =>
                //    c.Name.StartsWith(XSRF_TOKEN, StringComparison.OrdinalIgnoreCase));

                // Value of XSRF token cookie
                //antiForgerytoken.XsrfToken = _antiforgeryCookie.Value.ToString();

                // and the cookies unparsed (both XSRF-TOKEN and .AspNetCore.Antiforgery.{someId})
                antiForgerytoken.Cookies = values.ToArray();
            }

            return antiForgerytoken;
        }
    }

    internal class LoginRequestModel
    {
        public LoginRequestModel()
        {
        }

        public string Nick { get; set; }
        public string Contraseña { get; set; }
    }

    internal class LoginTokenRequestModel
    {
        public LoginTokenRequestModel()
        {
        }

        public string Token { get; set; }
    }

    public class AntiForgeryToken
    {
        public string XsrfToken { get; set; }
        public string[] Cookies { get; set; }
    }
}
