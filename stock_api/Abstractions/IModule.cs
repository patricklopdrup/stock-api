namespace stock_api.Abstractions
{
    public interface IModule
    {
        WebApplicationBuilder RegisterModule(WebApplicationBuilder builder);

        IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints);
    }
}
