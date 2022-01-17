namespace stock_api.Features.StockFeature
{
    public class StockModule : IModule
    {
        public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
        {


            return endpoints;
        }

        public WebApplicationBuilder RegisterModule(WebApplicationBuilder builder)
        {
            return builder;
        }
    }
}
