namespace SpiderEngine.Model;

public class SpiderTaskInfo
{
    public string Name { get; set; } = default!;
    public string Url { get; set; } = default!;
    public int? Success { get; set; }
    public int Delay { get; set; }
    public SpiderMethod Method { get; set; } = default!;
    public SpiderPayload? Payload { get; set; } = default!;
    public SpiderParserInfo? Content { get; set; } = default!;
    public IEnumerable<SpiderKeyPathPair>? Header { get; set; } = default!;
}

public enum SpiderMethod
{
    Get,
    Post,
}
