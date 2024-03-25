namespace SpiderEngine.Abstract
{
    using SpiderEngine.Model;

    public interface ISpider
    {
        IDictionary<string, string> Run(SpiderInfo spiderInfo, IDictionary<string, string> environment);
    }
}
