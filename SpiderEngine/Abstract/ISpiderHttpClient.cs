namespace SpiderEngine.Abstract
{
    using SpiderEngine.Model;

    public interface ISpiderHttpClient
    {
        Task<(string Content, IDictionary<string, string> Headers)> GetAsync(string url, IDictionary<string, string> headers, int success = 200);
        Task<(string Content, IDictionary<string, string> Headers)> PostAsync(string url, IDictionary<string, string> headers, string payload, SpiderPayloadType type, int success = 200);
    }

    public interface ISpiderHttpClientProvider
    {
        ISpiderHttpClient Create(EngineOptions options);
    }
}
