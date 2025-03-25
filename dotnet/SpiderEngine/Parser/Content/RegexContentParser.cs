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
        Regex.Match(content, patten);
        var matches = Regex.Matches(content, patten, RegexOptions.Singleline).ToList();

        try
        {
            var dict = matches
                .Select(
                    (match, index) =>
                    {
                        if (!match.Success)
                        {
                            throw new ParseException("Content does not match the patten.");
                        }
                        return (index, match.Groups.Values.First());
                    }
                )
                .ToDictionary();
            foreach (var pair in value)
            {
                Context[pair.Key] = dict[int.Parse(pair.Path)].Value;
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
