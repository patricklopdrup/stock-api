using Newtonsoft.Json.Linq;

namespace stock_api.Features.ExchangeFeatures.BinanceFeature
{
    public class BinanceModule : IModule
    {
        private BinanceHelper _binance = new BinanceHelper();

        public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/binance/savebalance", SaveBinanceBalance);

            return endpoints;
        }

        public WebApplicationBuilder RegisterModule(WebApplicationBuilder builder)
        {
            return builder;
        }


        #region CRUD methods
        internal async Task<IResult> SaveBinanceBalance(CustomDbContext db, int userId)
        {
            return await new BinanceHelper().SaveBalance(db, userId);
        }

        #endregion
    }
}
