using stock_api.Abstractions;

namespace stock_api.Extensions
{
    public static class ModuleExtension
    {
        private static List<IModule> _modules = new List<IModule>();

        public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder)
        {
            _modules = DiscoverModules();
            foreach (var module in _modules)
            {
                module.RegisterModule(builder);
            }

            return builder;
        }

        public static WebApplication MapEndpoints(this WebApplication app)
        {
            foreach (var module in _modules)
            {
                module.MapEndpoint(app);
            }

            return app;
        }

        private static List<IModule> DiscoverModules()
        {
            return typeof(IModule).Assembly
                .GetTypes()
                .Where(module => module.IsClass && module.IsAssignableTo(typeof(IModule)))
                .Select(Activator.CreateInstance)
                .Cast<IModule>()
                .ToList();
        }
    }
}
