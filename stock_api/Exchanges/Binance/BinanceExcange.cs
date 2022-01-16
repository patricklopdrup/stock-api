using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace stock_api.Exchanges.Binance
{
    public class BinanceExcange
    {
        // Binance API documentation: https://binance-docs.github.io/apidocs/#change-log

        private const string _baseUrl = "https://api.binance.com";
        private const string _apiKey = Credential.BinanceApiKey;
        private const string _secretKey = Credential.BinanceApiSecret;
        private HttpClient _httpClient = new HttpClient();


        public async Task<bool> AddBalanceToDatabase(CustomDbContext db)
        {
            JObject binanceAccount = await SendSignedRequest("/api/v3/account");
            var balance = binanceAccount["balances"];
            if (balance == null)
            {
                return false;
            }

            foreach (var asset in balance)
            {
                double amount = (double)asset["free"];
                if (amount > 0.0)
                {
                    string name = asset["asset"].ToString();
                    if (!await AddAssetToDatabase(name, amount, db))
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        public async Task<bool> AddAssetToDatabase(string name, double amount, CustomDbContext db)
        {
            double price = await GetPriceOfCrypto(name);

            Stock stock = new Stock
            {
                Name = name,
                Amount = amount,
                Type = StockType.Crypto,
                Price = price,
                Currency = "USD",
                UserId = 2
            };

            try
            {
                await db.Stocks.AddAsync(stock);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }



        public async Task<double> GetPriceOfCrypto(string name)
        {
            // Compare the price to the stable coin "USDT"
            string symbol = $"{name}USDT";
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
    }
}
