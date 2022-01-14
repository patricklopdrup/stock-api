using stock_api.Dal;

namespace stock_api.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<CustomDbContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            return services;
        }
    }
}
