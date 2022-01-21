using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using stock_api;
using stock_api.Features.ExchangeFeatures.BinanceFeature;
using stock_api.Features.ExchangeFeatures.NordnetFeature;
using System.Net;
using System.Text;

const string _apiKey = Credential.BinanceApiKey;
const string _secretKey = Credential.BinanceApiSecret;

BinanceHelper binance = new BinanceHelper();
NordnetHelper nordnet = new NordnetHelper();

//var hej = await binance.GetTimeStamp();
//Console.WriteLine(hej);

string _username = Credential.NordnetUsername;
string _password = Credential.NordnetPassword;

await nordnet.UpdateSessionCookie();

await nordnet.UpdateSessionCookie();

Console.ReadLine();