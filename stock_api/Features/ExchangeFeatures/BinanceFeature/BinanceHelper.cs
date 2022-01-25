using Newtonsoft.Json.Linq;
using stock_api.Features.StockFeature;
using System.Security.Cryptography;
using System.Text;

namespace stock_api.Features.ExchangeFeatures.BinanceFeature
{
    public class BinanceHelper : StockHelper
    {
        // Binance API documentation: https://binance-docs.github.io/apidocs/#change-log

        private const string _baseUrl = "https://api.binance.com";
        private const string _apiKey = Credential.BinanceApiKey;
        private const string _secretKey = Credential.BinanceApiSecret;
        private HttpClient _httpClient = new HttpClient();



        /// <summary>
        /// Get the price of a crypto in USD. This method compare the crypto to USDT.
        /// </summary>
        /// <param name="ticker">The name/ticker of the crypto you want the price of.</param>
        /// <example>For example:
        /// <code>
        /// double btcPrice = await GetPriceOfCrypto("btc");
        /// </code>
        /// results in <c>btcPrice</c> having the value 42495.00.
        /// </example>
        /// <returns>The price of the crypto in USD. -1.0 if an error occur.</returns>
        public async Task<double> GetPriceOfCrypto(string ticker)
        {
            // Compare the price to the stable coin "USDT"
            string symbol = $"{ticker}USDT";
            var result = await SendUnsignedRequest("/api/v3/ticker/price", symbol: symbol);
            if (result == null)
            {
                return -1.0;
            }

            return (double)result["price"];
        }



        /// <summary>
        ///  Send a request via Binance API which are hash signature hashed with the secret key and
        ///  the api key as a header.
        /// </summary>
        /// <param name="endpoint">The binance endpoint to reach. E.g. /api/v3/account.</param>
        /// <param name="payload"></param>
        /// <returns>A JObject of the JSON response.</returns>
        public async Task<JObject> SendSignedRequest(string endpoint, string? payload = null)
        {
            var serverTime = await GetTimeStamp();
            string query = $"timestamp={serverTime}";
            var signature = HashHMAC(query);
            string urlToSend = $"{_baseUrl + endpoint}?{query}&signature={signature}";

            _httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
            string result = "";
            HttpResponseMessage response = await _httpClient.GetAsync(urlToSend);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }

            return JObject.Parse(result);
        }


        public async Task<JObject> SendUnsignedRequest(string endpoint, string? body = null, string[] symbols = null)
        {
            if (symbols != null)
            {
                if (symbols.Length == 1)
                    endpoint += $"?symbol={symbols[0]}";
                else
                    endpoint += $"?symbols[{string.Join(',', symbols)}]";
            }
            string result = "";
            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + endpoint);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }

            return JObject.Parse(result);
        }

        public async Task<JObject> SendUnsignedRequest(string endpoint, string? body = null, string symbol = "")
        {
            return await SendUnsignedRequest(endpoint, body, new string[] { symbol });
        }


        /// <summary>
        /// Gets a timestamp used when sending request via the Binance API.
        /// </summary>
        /// <returns>A time in seconds.</returns>
        public async Task<long> GetTimeStamp()
        {
            string result = "";
            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + "/api/v3/time");
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }

            // Create a dynamic object to use the dot [.] operator
            var data = JObject.Parse(result);

            var time = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            long myTime = (long)time.TotalSeconds * 1000;
            long difference = myTime - (long)data["serverTime"];

            return myTime - difference;
        }


        /// <summary>
        /// Create a signature of the query and hash it with the secret key.
        /// </summary>
        /// <param name="query">The query to be hashed as a signature.</param>
        /// <returns>The signature as a hex string.</returns>
        public string HashHMAC(string query)
        {
            byte[] keyAsByteArray = Encoding.UTF8.GetBytes(_secretKey);
            byte[] queryAsByteArray = Encoding.UTF8.GetBytes(query);

            var hmac = new HMACSHA256(keyAsByteArray);
            return ToHexString(hmac.ComputeHash(queryAsByteArray));
        }

        /// <summary>
        /// Convert a byte array to a hex string.
        /// </summary>
        /// <param name="bytes">The byte array to convert.</param>
        /// <returns>A hex string.</returns>
        private string ToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                sb.AppendFormat("{0:x2}", b);

            return sb.ToString();
        }

        /// <summary>
        /// Get the non-zero Binance balance from the account.
        /// </summary>
        /// <returns>A JArray with the assets as JTokens.</returns>
        public async override Task<JArray> GetBalance()
        {
            JArray balance = new JArray();
            JObject binanceAccount = await SendSignedRequest("/api/v3/account");
            var accountBalance = binanceAccount["balances"];
            if (accountBalance == null)
            {
                return new JArray();
            }

            foreach (var asset in accountBalance)
            {
                double amount = (double)asset["free"];
                if (amount > 0.0)
                    balance.Add(asset);
            }

            return balance;
        }


        /// <summary>
        /// Get a Stock object populated as a crypto stock.
        /// </summary>
        /// <param name="asset">The asset to make into a Stock object.</param>
        /// <param name="userId">The ID of the user who owns the stock.</param>
        /// <returns>A populated Stock object</returns>
        internal override Stock GetDefaultStock(JToken asset, int userId)
        {
            string name = asset["asset"].ToString();
            Stock stock = new Stock
            {
                Ticker = name,
                Name = name,
                Type = StockType.Crypto,
                Currency = "USD",
                UserId = userId
            };

            return stock;
        }


        /// <summary>
        /// Get a DailyPrice crypto object for a stock object.
        /// </summary>
        /// <param name="stock">The ticker of the stock we update for.</param>
        /// <param name="asset">The JToken object of the asset to update.</param>
        /// <returns>A DailyPrice object for a crypto stock.</returns>
        internal async override Task<DailyPrice> GetUpdateStock(JToken asset, Stock stock)
        {
            double price = await GetPriceOfCrypto(stock.Ticker);
            double amount = (double)asset["free"];

            DailyPrice updateStock = new DailyPrice()
            {
                Price = price,
                Amount = amount,
                StockTicker = stock.Ticker
            };

            return updateStock;
        }
    }
}
