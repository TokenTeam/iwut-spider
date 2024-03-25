namespace SpiderEngine.Parser.Payload
{
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Json;
    using SpiderEngine.Abstract;
    using SpiderEngine.Model;
    using SpiderEngine.Utility;

    internal class JsonPayloadParser : IPayloadParser
    {
        public IDictionary<string, string> Context { get; set; } = default!;

        public string Parse(string patten, IEnumerable<SpiderKeyValuePair> value)
        {
            using var stream = new MemoryStream();
            var json = new Utf8JsonWriter(stream);

            json.WriteStartObject();
            foreach (var pair in value)
            {
                json.WriteStartObject(pair.Key);
                var valueExpression = pair.Value.FillVariables(Context);

                switch (pair.Type)
                {
                    case SpiderValueType.String:
                        json.WriteStringValue(valueExpression);
                        break;
                    case SpiderValueType.Number:
                        json.WriteNumberValue(double.Parse(valueExpression));
                        break;
                    case SpiderValueType.Boolean:
                        json.WriteBooleanValue(bool.Parse(valueExpression));
                        break;
                    case SpiderValueType.Object:
                        json.WriteRawValue(valueExpression);
                        break;
                }
                json.WriteEndObject();
            }
            json.WriteEndObject();
            json.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
