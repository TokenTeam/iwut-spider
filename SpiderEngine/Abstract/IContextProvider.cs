namespace SpiderEngine.Abstract
{
    internal interface IContextProvider
    {
        IDictionary<string, string> Context { get; set; }
    }
}
