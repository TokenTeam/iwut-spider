namespace SpiderEngine.Test
{
    using SpiderEngine.Abstract;
    using SpiderEngine.Model;

    public static class Config
    {
        public static readonly string ApiBaseUrl = "http://localhost:5000/api";
        public static readonly string CasCasesDir = "../../../../../test/cases/iwut";

        public static readonly string UserName = "*************";
        public static readonly string Password = "*********";

        public static string CasCasePath(string name)
        {
            return $"{CasCasesDir}/{name}.json";
        }

        public static SpiderInfo ReadCaseFromFile(this ISpider spider, string file)
        {
            var json = File.ReadAllText(file);
            return spider.Deserialize(json);
        }
    }
}
