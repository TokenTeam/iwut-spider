namespace SpiderEngine.Abstract;

using SpiderEngine.Model;

public interface ISpider
{
    ISpiderHttpClient CreateClient(SpiderInfo spiderInfo);
    IDictionary<string, string> Run(SpiderInfo spiderInfo, IDictionary<string, string> environment, ISpiderHttpClient? client = null);
    SpiderInfo Deserialize(string json);
    string Serialize(SpiderInfo spiderInfo);
    string GetSchema();
}
