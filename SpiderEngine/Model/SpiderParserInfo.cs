namespace SpiderEngine.Model;

public class SpiderParserInfo
{
    public SpiderParserType Type { get; set; } = default!;
    public string Patten { get; set; } = default!;
    public IEnumerable<SpiderKeyPathPair> Value { get; set; } = default!;
}

public enum SpiderParserType
{
    Regex,
    Json,
}
