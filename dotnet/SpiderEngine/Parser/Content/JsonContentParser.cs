namespace SpiderEngine.Parser.Content;

using System.Collections.Generic;
using System.Text.Json;
using SpiderEngine.Abstract;
using SpiderEngine.Exception;
using SpiderEngine.Model;

internal class JsonContentParser : IContentParser
{
    public IDictionary<string, string> Context { get; set; } = default!;

    public void Parse(string content, string patten, IEnumerable<SpiderKeyPathPair> value)
    {
        try
        {
            JsonDocument document = JsonDocument.Parse(content);
            foreach (var pair in value)
            {
                JsonElement element = document.RootElement;
                foreach (var path in pair.Path.Split('.'))
                {
                    if (element.ValueKind == JsonValueKind.Array)
                    {
                        element = element[int.Parse(path)];
                    }
                    else
                    {
                        element = element.GetProperty(path);
                    }
                }
                Context[pair.Key] = element.ToString() ?? string.Empty;
            }
        }
        catch (JsonException ex)
        {
            throw new ParseException("Json parse failed", ex);
        }
        catch (AggregateException ex)
        {
            throw new ParseException("Json parse failed", [.. ex.InnerExceptions]);
        }
        catch (System.Exception ex)
        {
            throw new ParseException("Json parse failed", ex);
        }
    }
}
