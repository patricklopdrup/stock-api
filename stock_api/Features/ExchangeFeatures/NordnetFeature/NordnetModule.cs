
namespace stock_api.Features.ExchangeFeatures.NordnetFeature
{
    public class NordnetModule : IModule
    {
        private NordnetHelper _nordnetHelper = new NordnetHelper();

        public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/nordnet/savebalance", SaveNordnetBalance);

            return endpoints;
        }

        public WebApplicationBuilder RegisterModule(WebApplicationBuilder builder)
        {
            return builder;
        }


        #region CRUD methods
        internal async Task<IResult> SaveNordnetBalance(CustomDbContext db, int userId)
        {
            return await new NordnetHelper().SaveBalance(db, userId);
        }

        #endregion
    }
}
