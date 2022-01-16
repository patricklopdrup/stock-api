using stock_api.Exchanges.Binance;

namespace stock_api.Features.ExchangeFeatures.BinanceFeature
{
    public class BinanceModule : IModule
    {
        private BinanceExcange _binance = new BinanceExcange();

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
        // TODO: refactor her og måske bare smid BinanceExchange.cs ind i feature mappe
        // og brug den som helper methods. Og alt, som har noget med db at gøre er i denne fil.
        internal async Task<IResult> SaveBalance(CustomDbContext db)
        {
            if (await _binance.AddBalanceToDatabase(db))
            {
                return Results.Ok();
            } else
            {
                return Results.NotFound();
            }
        }

        #endregion
    }
}
