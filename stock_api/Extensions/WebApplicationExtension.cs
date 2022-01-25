using stock_api.Dal;
using stock_api.Models;

namespace stock_api.Extensions
{
    public static class WebApplicationExtension
    {
        public static WebApplication ConfigureApplication(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            return app;
        }
    }
}
