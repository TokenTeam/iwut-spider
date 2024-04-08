namespace SpiderEngine.Test.CasTest
{
    using SpiderEngine.Abstract;
    using SpiderEngine.Service;

    public class CasGetPubkey
    {
        public CasGetPubkey()
        {
            _spider = new Spider(new DefaultSpiderHttpClientProvider());
        }

        private readonly ISpider _spider;

        [Fact]
        public void GetPubkey()
        {
            var spiderInfo = _spider.ReadCaseFromFile(Config.CasCasePath("ias_get_public_key"));
            var output = _spider.Run(spiderInfo, new Dictionary<string, string>());
            var pubkey = output["pubkey"];
            Assert.NotNull(pubkey);
        }
    }
}
