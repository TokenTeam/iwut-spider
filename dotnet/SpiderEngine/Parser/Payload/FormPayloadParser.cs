namespace SpiderEngine.Parser.Payload;

using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using SpiderEngine.Abstract;
using SpiderEngine.Model;
using SpiderEngine.Utility;

internal class FormPayloadParser : IPayloadParser
{
    public IDictionary<string, string> Context { get; set; } = default!;

    public string Parse(string patten, IEnumerable<SpiderKeyValuePair> value)
    {
        if (value is null || !value.Any())
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (var pair in value)
        {
            var valueExpression = pair.Value.FillVariables(Context);
            sb.Append(pair.Key).Append('=').Append(UrlEncoder.Default.Encode(valueExpression)).Append('&');
        }
        return sb.Remove(sb.Length - 1, 1).ToString();
    }
}
