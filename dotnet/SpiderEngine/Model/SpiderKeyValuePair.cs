namespace SpiderEngine.Model;

public class SpiderKeyValuePair
{
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
    public SpiderValueType Type { get; set; }
}

public enum SpiderValueType
{
    String,
    Number,
    Boolean,
    Object,
}
