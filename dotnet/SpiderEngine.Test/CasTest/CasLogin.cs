namespace SpiderEngine.Test.CasTest
{
    using System.Security.Cryptography;
    using System.Text;
    using SpiderEngine.Abstract;
    using SpiderEngine.Service;

    public class CasLogin
    {
        public CasLogin()
        {
            _spider = new Spider(new DefaultSpiderHttpClientProvider());
        }

        private readonly ISpider _spider;

        [Fact]
        public void Login()
        {
            var env = new Dictionary<string, string>();
            var spiderGetPubkey = _spider.ReadCaseFromFile(Config.CasCasePath("ias_get_public_key"));
            var output = _spider.Run(spiderGetPubkey, env);

            var pubkey = output["pubkey"];

            var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(pubkey), out _);

            var ul = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(Config.UserName), RSAEncryptionPadding.Pkcs1));
            var pl = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(Config.Password), RSAEncryptionPadding.Pkcs1));

            var spiderLogin = _spider.ReadCaseFromFile(Config.CasCasePath("ias_login_redirect"));
            env["ul"] = ul;
            env["pl"] = pl;
            env["timestamp"] = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            output = _spider.Run(spiderLogin, env);
            Assert.NotNull(output);
        }
    }
}
