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

            ICollection<Stock> newStocks = new List<Stock>();
            ICollection<DailyPrice> stocksToUpdate = new List<DailyPrice>();
            foreach (var asset in balance)
            {
                Stock stock = _binance.GetDefaultCryptoStock(asset, 1);

                if (stock.IsNewInPortfolio(db))
                {
                    newStocks.Add(stock);
                    DailyPrice updateFirstTime = await _binance.GetUpdateCryptoStock(stock);
                    stocksToUpdate.Add(updateFirstTime);
                }
                else
                {
                    DailyPrice updateStock = await _binance.GetUpdateCryptoStock(stock);
                    stocksToUpdate.Add(updateStock);
                }
            }

            try
            {
                var addUpdate = db.DailyPrices.AddRangeAsync(stocksToUpdate);
                var addNew = db.Stocks.AddRangeAsync(newStocks);
                await Task.WhenAll(addUpdate, addNew);

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // log error
                return Results.BadRequest(ex.Message);
            }

            return balance.Count == stocksToUpdate.Count
                ? Results.Ok()
                : Results.Problem(detail: $"Only updated {stocksToUpdate.Count}/{balance.Count} assets to DB.");
        }

        #endregion
    }
}
