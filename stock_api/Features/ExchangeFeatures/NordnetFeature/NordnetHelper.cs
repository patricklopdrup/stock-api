using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace stock_api.Features.ExchangeFeatures.NordnetFeature
{
    public class NordnetHelper
    {
        /// Nordnet api: https://www.nordnet.dk/externalapi/docs/api
        /// Brug basic auth med brugernavn og password
        /// Få først cookie ud og send den med i de andre requests i headeren.
        /// Brug også header: client-id : NEXT
        /// Og Accept-Language : en
        /// Til at få de forskellige konti: https://www.nordnet.se/api/2/accounts
        /// Herefter brug de account id'er til at få indholdet ud: https://www.nordnet.se/api/2/accounts/1/positions

        // Nordnet API documentation: https://www.nordnet.dk/externalapi/docs/api

        private readonly string _baseUrl = "https://www.nordnet.dk/api/2";
        private string _username = Credential.NordnetUsername;
        private string _password = Credential.NordnetPassword;
        private HttpClient _httpClient;

        public static string SessionCookie;

        /// <summary>
        /// Login to Nordnet and get a NEXT session cookie to be used with other API calls.
        /// </summary>
        /// <returns>A NEXT session cookie as a string. E.g. NEXT=5c75de177fb86g46e9e9c3463q0f26864f70835c</returns>
        public async Task<string> GetSessionCookie()
        {
            CookieContainer cookieContainer = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            _httpClient = new HttpClient(handler);
            CookieCollection cookies = new CookieCollection();
            handler.CookieContainer = cookieContainer;

            // Set headers for Nordnet
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en");
            _httpClient.DefaultRequestHeaders.Add("client-id", "NEXT");
            _httpClient.DefaultRequestHeaders.Add("sub-client-id", "NEXT");
            _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");

            // First login part to get initial cookies
            var url1 = new Uri($"{_baseUrl}/login/anonymous");
            var res1 = await _httpClient.PostAsync(url1, null);
            if (res1.IsSuccessStatusCode)
            {
                cookies = cookieContainer.GetCookies(url1);
            }

            // Actual login to get the session-key/session cookie. E.g. "NEXT=34jlh3jhfjht4"
            var url2 = new Uri($"{_baseUrl}/authentication/basic/login");

            // Create a JSON object with username and password
            dynamic info = new JObject();
            info.username = _username;
            info.password = _password;
            var json = JsonConvert.SerializeObject(info); 
            var payload = new StringContent(json, Encoding.UTF8, "application/json");

            cookieContainer.Add(url2, cookies);
            var res3 = await _httpClient.PostAsync(url2, payload);
            if (res3.IsSuccessStatusCode)
            {
                var content = await res3.Content.ReadAsStringAsync();
                var jsonRes = JObject.Parse(content);
                if (jsonRes.ContainsKey("session_key"))
                {
                    var sessionCookie = "NEXT=" + jsonRes["session_key"].ToString();
                    SessionCookie = sessionCookie;
                    return sessionCookie;
                }
                else
                {
                    return "";
                }
            }

            return "";
        }


        

    }
}
