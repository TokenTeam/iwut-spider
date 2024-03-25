namespace SpiderEngine.Abstract
{
    using SpiderEngine.Model;
    using SpiderEngine.Parser.Payload;

    internal interface IPayloadParser : IContextProvider
    {
        string Parse(string patten, IEnumerable<SpiderKeyValuePair> value);

        public static IPayloadParser Create(SpiderPayloadType type) => type switch
        {
            SpiderPayloadType.Text => throw new NotImplementedException(),
            SpiderPayloadType.Json => new JsonPayloadParser(),
            SpiderPayloadType.Form => new FormPayloadParser(),
            SpiderPayloadType.Params => new FormPayloadParser(),
            _ => throw new NotImplementedException(),
        };
    }
}
