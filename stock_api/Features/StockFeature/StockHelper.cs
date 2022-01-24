using Newtonsoft.Json.Linq;

namespace stock_api.Features.StockFeature
{
    public abstract class StockHelper
    {
        internal async Task<IResult> SaveBalance(CustomDbContext db, int userId)
        {
            var balance = await GetBalance();
            if (balance == null) return Results.BadRequest();

            ICollection<Stock> newStocks = new List<Stock>();
            ICollection<DailyPrice> stocksToUpdate = new List<DailyPrice>();

            foreach (var asset in balance)
            {
                Stock stock = GetDefaultStock(asset, userId);

                // Only add a new stock the first time it is seen
                if (stock.IsNewInPortfolio(db))
                {
                    newStocks.Add(stock);
                }
                // Update always
                DailyPrice updateStock = await GetUpdateStock(asset, stock);
                stocksToUpdate.Add(updateStock);
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal abstract Task<JArray> GetBalance();
        internal abstract Stock GetDefaultStock(JToken asset, int userId);
        internal abstract Task<DailyPrice> GetUpdateStock(JToken asset, Stock stock);
    }
}
