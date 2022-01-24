using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using stock_api.Features.StockFeature;
using System.Net;
using System.Text;

namespace stock_api.Features.ExchangeFeatures.NordnetFeature
{
    public class NordnetHelper : StockHelper
    {
        /// Nordnet api: https://www.nordnet.dk/externalapi/docs/api
        /// Brug basic auth med brugernavn og password
        /// Få først cookie ud og send den med i de andre requests i headeren.
        /// Brug også header: client-id : NEXT
        /// Og Accept-Language : en
        /// Til at få de forskellige konti: https://www.nordnet.se/api/2/accounts
        /// Herefter brug de account id'er til at få indholdet ud: https://www.nordnet.se/api/2/accounts/1/positions

        // Nordnet API documentation: https://www.nordnet.dk/externalapi/docs/api

        public NordnetHelper()
        {
            _handler.CookieContainer = _cookieContainer;
        }

        private readonly string _baseUrl = "https://www.nordnet.dk/api/2";
        private string _username = Credential.NordnetUsername;
        private string _password = Credential.NordnetPassword;

        private static CookieContainer _cookieContainer = new CookieContainer();
        private static HttpClientHandler _handler = new HttpClientHandler();
        private static HttpClient _httpClient = new HttpClient(_handler);

        /// CookieCollection which holds the cookies from <see cref="NordnetHelper.UpdateSessionCookie"/>.
        private static CookieCollection _cookieJar = new CookieCollection();

        /// <summary>
        /// Login to Nordnet and update the cookieCollection with NEXT session cookie to be used with other API calls.
        /// </summary>
        public async Task UpdateSessionCookie()
        {
            // Do not login again if a session is already up and running
            if (await IsCookieCollectionValid())
                return;

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
                _cookieJar = _cookieContainer.GetCookies(url1);
            }

            // Actual login to get the session-key/session cookie. E.g. "NEXT=34jlh3jhfjht4"
            var url2 = new Uri($"{_baseUrl}/authentication/basic/login");

            // Create a JSON object with username and password
            dynamic info = new JObject();
            info.username = _username;
            info.password = _password;
            var json = JsonConvert.SerializeObject(info); 
            var payload = new StringContent(json, Encoding.UTF8, "application/json");

            _cookieContainer.Add(url2, _cookieJar);
            var res2 = await _httpClient.PostAsync(url2, payload);
            if (res2.IsSuccessStatusCode)
            {
                // Update the cookieCollection to be used by other API calls
                _cookieJar = _cookieContainer.GetCookies(url2);
                if (_cookieJar.Any(cookie => cookie.Name == "NEXT"))
                {
                    // Log that the session cookie has been updated
                }
                else
                {
                    // Log error; NEXT cookie not in cookieJar
                }
            }
            else
            {
                // Log error; res2 not successful
            }
        }


        /// <summary>
        /// Check if a session is valid. If this is true there is no need to login again.
        /// </summary>
        /// <returns>True if a session is valid; false otherwise.</returns>
        private async Task<bool> IsCookieCollectionValid()
        {
            // Basic API call that requires a valid NEXT session cookie
            var checkUrl = new Uri($"{_baseUrl}/accounts");
            _cookieContainer.Add(checkUrl, _cookieJar);
            var res = await _httpClient.GetAsync(checkUrl);
            if (res.IsSuccessStatusCode)
            {
                // Log that a session is already up
            }
            else
            {
                // Log that a new session is going to be made
            }

            return res.IsSuccessStatusCode;
        }


        public async Task<ICollection<int>> GetAllAccountIds()
        {
            await UpdateSessionCookie();

            var url = new Uri($"{_baseUrl}/accounts");
            _cookieContainer.Add(url, _cookieJar);
            ICollection<int> accountIds = new List<int>();
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonArray = JArray.Parse(content);
                foreach (var account in jsonArray)
                    accountIds.Add((int)account["accid"]);
            }

            return accountIds;
        }

        internal async override Task<JArray> GetBalance()
        {
            await UpdateSessionCookie();

            var allPositionsJson = new JArray();
            var accountIds = await GetAllAccountIds();
            foreach (var accountId in accountIds)
            {
                var url = new Uri($"{_baseUrl}/accounts/{accountId}/positions");
                _cookieContainer.Add(url, _cookieJar);
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    // Parse the content into a JSON array
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonArray = JArray.Parse(content);

                    // Merge posisitions for each account
                    allPositionsJson.Merge(jsonArray);
                }
                else
                {
                    // log error
                }
            }

            return allPositionsJson;
        }

        internal override Stock GetDefaultStock(JToken asset, int userId)
        {
            var instrument = asset["instrument"];
            // If an instrument has "underlyings" it means that it is a fund otherwise a single share
            var stockType = instrument["underlyings"] is null ? StockType.Share : StockType.Fund;

            Stock stock = new Stock()
            {
                Ticker = instrument["symbol"].ToString(),
                Name = instrument["name"].ToString(),
                Currency = instrument["currency"].ToString(),
                IsinCode = instrument["isin_code"].ToString(),
                Type = stockType,
                UserId = userId
            };

            return stock;
        }

        internal override Task<DailyPrice> GetUpdateStock(JToken asset, Stock stock)
        {
            DailyPrice updateStock = new DailyPrice()
            {
                Price = (double)asset["main_market_price"]["value"],
                Amount = (double)asset["qty"],
                PurchasePrice = (double)asset["acq_price_acc"]["value"],
                OpenPrice = (double)asset["morning_price"]["value"],
                StockTicker = stock.Ticker
            };

            return Task.Run(() => updateStock);
        }
    }
}
