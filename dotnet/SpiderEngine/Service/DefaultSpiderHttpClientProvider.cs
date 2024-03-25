namespace SpiderEngine.Service
{
    using SpiderEngine.Abstract;
    using SpiderEngine.Http;
    using SpiderEngine.Model;

    internal class DefaultSpiderHttpClientProvider : ISpiderHttpClientProvider
    {
        public ISpiderHttpClient Create(EngineOptions options) => new SpiderHttpClient(options);
    }
}
