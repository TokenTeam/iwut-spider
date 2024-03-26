namespace SpiderEngine.Abstract;

using SpiderEngine.Model;
using SpiderEngine.Parser.Content;

internal interface IContentParser : IContextProvider
{
    void Parse(string content, string patten, IEnumerable<SpiderKeyPathPair> value);

    public static IContentParser Create(SpiderParserType type) => type switch
    {
        SpiderParserType.Json => new JsonContentParser(),
        SpiderParserType.Regex => new RegexContentParser(),
        _ => throw new NotImplementedException(),
    };
}
