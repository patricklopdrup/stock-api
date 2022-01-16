using stock_api;
using stock_api.Exchanges.Binance;

const string _apiKey = Credential.BinanceApiKey;
const string _secretKey = Credential.BinanceApiSecret;

BinanceExcange binance = new BinanceExcange();

var hej = await binance.GetTimeStamp();
Console.WriteLine(hej);


Console.ReadLine();