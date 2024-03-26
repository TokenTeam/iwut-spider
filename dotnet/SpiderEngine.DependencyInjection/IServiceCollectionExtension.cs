namespace SpiderEngine.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;
    using SpiderEngine;
    using SpiderEngine.Abstract;
    using SpiderEngine.Service;

    public static class IServiceCollectionExtension
    {
        public static void AddSpiderEngine(this IServiceCollection services)
        {
            services.AddSingleton<ISpider, Spider>();
            services.AddSingleton<ISpiderHttpClientProvider, DefaultSpiderHttpClientProvider>();
        }
    }
}
