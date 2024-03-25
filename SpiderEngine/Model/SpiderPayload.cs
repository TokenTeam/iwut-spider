namespace SpiderEngine.Model;

public class SpiderPayload
{
    public SpiderPayloadType Type { get; set; } = default!;
    public string Patten { get; set; } = default!;
    public IEnumerable<SpiderKeyValuePair> Value { get; set; } = default!;
    public IEnumerable<SpiderKeyValuePair>? Header { get; set; } = default!;
}

public enum SpiderPayloadType
{
    Text,
    Json,
    Form,
    Params,
}