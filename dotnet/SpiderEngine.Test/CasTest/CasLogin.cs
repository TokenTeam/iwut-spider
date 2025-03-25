namespace SpiderEngine.Test.CasTest;

using SpiderEngine.Abstract;
using SpiderEngine.Service;
using SpiderEngine.Test.Model;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

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

        var spiderLogin = _spider.ReadCaseFromFile(Config.CasCasePath("ias_login_home"));
        env["ul"] = ul;
        env["pl"] = pl;
        env["redirect_url"] = UrlEncoder.Default.Encode("https://zhlgd.whut.edu.cn/tp_up/");
        output = _spider.Run(spiderLogin, env);
        Assert.NotNull(output);
    }

    [Fact]
    public void LoginAndGetGrade()
    {
        var env = new Dictionary<string, string>();
        var spiderGetPubkey = _spider.ReadCaseFromFile(Config.CasCasePath("ias_get_public_key"));
        var client = _spider.CreateClient(spiderGetPubkey);
        var output1 = _spider.Run(spiderGetPubkey, env, client);
        var pubkey = output1["pubkey"];

        Assert.NotEmpty(pubkey);

        var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(pubkey), out _);

        var ul = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(Config.UserName), RSAEncryptionPadding.Pkcs1));
        var pl = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(Config.Password), RSAEncryptionPadding.Pkcs1));

        var spiderLogin = _spider.ReadCaseFromFile(Config.CasCasePath("ias_login_home"));

        env["ul"] = ul;
        env["pl"] = pl;
        env["redirect_url"] = UrlEncoder.Default.Encode("https://zhlgd.whut.edu.cn/tp_up/");
        var output2 = _spider.Run(spiderLogin, env, client);
        Assert.NotNull(output2["zhlgd_home"]);

        var spiderGrade = _spider.ReadCaseFromFile(Config.CasCasePath("zhlgd_get_grade"));
        var output3 = _spider.Run(spiderGrade, env, client);
        Assert.NotNull(output3["grade_list"]);

        var grades = JsonSerializer.Deserialize<ZhlgdGrade[]>(output3["grade_list"]);
        Assert.NotEmpty(grades!);
    }

    [Fact]
    public void LoginAndGetLibraryInfo()
    {
        var env = new Dictionary<string, string>();
        var spiderGetPubkey = _spider.ReadCaseFromFile(Config.CasCasePath("library/ias_get_public_key"));
        var client = _spider.CreateClient(spiderGetPubkey);
        var output1 = _spider.Run(spiderGetPubkey, env, client);
        var pubkey = output1["pubkey"];

        Assert.NotEmpty(pubkey);

        var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(pubkey), out _);

        var ul = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(Config.UserName), RSAEncryptionPadding.Pkcs1));
        var pl = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(Config.Password), RSAEncryptionPadding.Pkcs1));

        var spiderLogin = _spider.ReadCaseFromFile(Config.CasCasePath("library/ias_library"));

        env["ul"] = ul;
        env["pl"] = pl;
        env["redirect_url"] = UrlEncoder.Default.Encode("http://202.114.89.11/opac/special/toOpac");
        var output2 = _spider.Run(spiderLogin, env, client);
        Assert.NotNull(output2["lib_home"]);
        Assert.NotEmpty(output2["lib_home"]);
    }
}
