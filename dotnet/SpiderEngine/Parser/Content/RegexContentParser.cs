namespace SpiderEngine.Parser.Content;

using System.Collections.Generic;
using System.Text.RegularExpressions;
using SpiderEngine.Abstract;
using SpiderEngine.Exception;
using SpiderEngine.Model;

internal class RegexContentParser : IContentParser
{
    public IDictionary<string, string> Context { get; set; } = default!;

    public void Parse(string content, string patten, IEnumerable<SpiderKeyPathPair> value)
    {
        var match = Regex.Match(content, patten);
        if (!match.Success)
        {
            throw new ParseException("Content does not match the patten.");
        }
        try
        {
            foreach (var pair in value)
            {
                Context[pair.Key] = match.Groups[int.Parse(pair.Path)].Value;
            }
        }
        catch (AggregateException ex)
        {
            throw new ParseException("Regex parse failed", [.. ex.InnerExceptions]);
        }
        catch (System.Exception ex)
        {
            throw new ParseException("Content does not match the patten.", ex);
        }
    }
}
