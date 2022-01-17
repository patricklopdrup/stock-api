using stock_api;
using stock_api.Features.ExchangeFeatures.BinanceFeature;

const string _apiKey = Credential.BinanceApiKey;
const string _secretKey = Credential.BinanceApiSecret;

BinanceHelper binance = new BinanceHelper();

var hej = await binance.GetTimeStamp();
Console.WriteLine(hej);


Console.ReadLine();