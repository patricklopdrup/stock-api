using Newtonsoft.Json.Linq;

namespace stock_api.Features.ExchangeFeatures.BinanceFeature
{
    public class BinanceModule : IModule
    {
        private BinanceHelper _binance = new BinanceHelper();

        public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/binance/savebalance", SaveBalance);

            return endpoints;
        }

        public WebApplicationBuilder RegisterModule(WebApplicationBuilder builder)
        {
            return builder;
        }


        #region CRUD methods
        internal async Task<IResult> SaveBalance(CustomDbContext db)
        {
            var balance = await _binance.GetBalance();
            if (balance == null) return Results.BadRequest();

            ICollection<Stock> stocksToAdd = new List<Stock>();
            foreach (var asset in balance)
            {
                Stock stock = await _binance.GetDefaultCryptoStock(asset, 2);
                // -1.0 on error
                if (stock.Price != -1.0)
                    stocksToAdd.Add(stock);
                else
                {
                    // log error
                }
            }

            try
            {
                await db.Stocks.AddRangeAsync(stocksToAdd);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // log error
                return Results.BadRequest(ex.Message);
            }

            return balance.Count == stocksToAdd.Count
                ? Results.Ok()
                : Results.Problem(detail: $"Only added {stocksToAdd.Count}/{balance.Count} assets to DB.");
        }

        #endregion
    }
}
