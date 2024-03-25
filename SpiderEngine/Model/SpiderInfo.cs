namespace SpiderEngine.Model;

public class SpiderInfo
{
    public string Name { get; set; } = default!;
    public int Version { get; set; }
    public IEnumerable<string> Environment { get; set; } = default!;
    public EngineOptions Engine { get; set; } = default!;
    public IEnumerable<SpiderTaskInfo> Task { get; set; } = default!;
    public IEnumerable<string> Output { get; set; } = default!;
}
