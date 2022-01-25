using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using stock_api;
using stock_api.Features.ExchangeFeatures.BinanceFeature;
using stock_api.Features.ExchangeFeatures.NordnetFeature;
using System.Net;
using System.Text;
using System.Net.Http.Headers;

const string _apiKey = Credential.BinanceApiKey;
const string _secretKey = Credential.BinanceApiSecret;

BinanceHelper binance = new BinanceHelper();
NordnetHelper nordnet = new NordnetHelper();

var saxoToken = Credential.SaxoDayToken;
var httpClient = new HttpClient();

var url = new Uri("https://gateway.saxobank.com/sim/openapi/port/v1/balances/me");
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", saxoToken);
var response = await httpClient.GetAsync(url);
if (response.IsSuccessStatusCode)
{
    var content = await response.Content.ReadAsStringAsync();
}





Console.ReadLine();