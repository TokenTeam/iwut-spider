namespace SpiderEngine.Utility;

using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
using SpiderEngine.Model;

internal static class SpiderSchema
{
    static SpiderSchema()
    {
        var generator = new JSchemaGenerator
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };
        generator.GenerationProviders.Add(new StringEnumGenerationProvider() { CamelCaseText = true });
        var schema = generator.Generate(typeof(SpiderInfo));
        _schema = schema.ToString();
    }

    private static readonly string _schema;

    public static string GetSchema()
    {
        return _schema;
    }
}
