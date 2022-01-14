using System.Security.Cryptography;
using System.Text;

namespace stock_api.Exchanges.Binance
{
    public class Binance
    {
        private const string _baseUrl = "https://api.binance.com/";
        private const string _apiKey = Credential.BinanceApiKey;
        private const string _secretKey = Credential.BinanceApiSecret;

        private byte[] HashHMAC(string key, string query)
        {
            byte[] keyAsByteArray = Encoding.ASCII.GetBytes(key);
            byte[] queryAsByteArray = Encoding.ASCII.GetBytes(query);

            var hmac = new HMACSHA256(keyAsByteArray);
            return hmac.ComputeHash(queryAsByteArray);
        }
    }
}
