namespace SpiderEngine.Test.CasTest;

using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using SpiderEngine.Abstract;
using SpiderEngine.Service;
using SpiderEngine.Test.Model;

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

        var ul = Convert.ToBase64String(
            rsa.Encrypt(Encoding.UTF8.GetBytes(Config.UserName), RSAEncryptionPadding.Pkcs1)
        );
        var pl = Convert.ToBase64String(
            rsa.Encrypt(Encoding.UTF8.GetBytes(Config.Password), RSAEncryptionPadding.Pkcs1)
        );

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

        var ul = Convert.ToBase64String(
            rsa.Encrypt(Encoding.UTF8.GetBytes(Config.UserName), RSAEncryptionPadding.Pkcs1)
        );
        var pl = Convert.ToBase64String(
            rsa.Encrypt(Encoding.UTF8.GetBytes(Config.Password), RSAEncryptionPadding.Pkcs1)
        );

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
        var spiderGetPubkey = _spider.ReadCaseFromFile(Config.CasCasePath("ias_get_public_key"));
        var client = _spider.CreateClient(spiderGetPubkey);
        var output1 = _spider.Run(spiderGetPubkey, env, client);
        var pubkey = output1["pubkey"];

        Assert.NotEmpty(pubkey);

        var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(pubkey), out _);

        var ul = Convert.ToBase64String(
            rsa.Encrypt(Encoding.UTF8.GetBytes(Config.UserName), RSAEncryptionPadding.Pkcs1)
        );
        var pl = Convert.ToBase64String(
            rsa.Encrypt(Encoding.UTF8.GetBytes(Config.Password), RSAEncryptionPadding.Pkcs1)
        );

        var spiderLogin = _spider.ReadCaseFromFile(Config.CasCasePath("library_login_home"));

        env["ul"] = ul;
        env["pl"] = pl;
        env["redirect_url"] = UrlEncoder.Default.Encode("http://202.114.89.11/opac/special/toOpac");
        var output2 = _spider.Run(spiderLogin, env, client);
        Assert.NotNull(output2["lib_home"]);
        Assert.NotEmpty(output2["lib_home"]);
    }

    [Fact]
    public void LoginAndGetZBKTInfo()
    {
        var env = new Dictionary<string, string>();
        var spiderGetPubkey = _spider.ReadCaseFromFile(
            Config.CasCasePath("ias_get_public_key")
        );
        var client = _spider.CreateClient(spiderGetPubkey);
        var output1 = _spider.Run(spiderGetPubkey, env, client);
        var pubkey = output1["pubkey"];

        Assert.NotEmpty(pubkey);

        var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(pubkey), out _);

        var ul = Convert.ToBase64String(
            rsa.Encrypt(Encoding.UTF8.GetBytes(Config.UserName), RSAEncryptionPadding.Pkcs1)
        );
        var pl = Convert.ToBase64String(
            rsa.Encrypt(Encoding.UTF8.GetBytes(Config.Password), RSAEncryptionPadding.Pkcs1)
        );

        var spiderLogin = _spider.ReadCaseFromFile(Config.CasCasePath("smart_login_home"));
        var r = UrlEncoder.Default.Encode("auth/login");
        var forward = UrlEncoder.Default.Encode("https://classroom.lgzk.whut.edu.cn/");
        var redirect_url = UrlEncoder.Default.Encode(
            $"https://yjapi.lgzk.whut.edu.cn/casapi/index.php?forward={forward}&r={r}&tenant_code=223"
        );
        var redirect_url_login = UrlEncoder.Default.Encode(
            "https://yjapi.lgzk.whut.edu.cn/casapi/index.php?forward=https://classroom.lgzk.whut.edu.cn/&r=auth/login&tenant_code=223"
        );
        env["ul"] = ul;
        env["pl"] = pl;
        env["redirect_url"] = redirect_url;
        env["redirect_url_login"] = redirect_url_login;
        env["date"] = "2025-03-24";
        var output2 = _spider.Run(spiderLogin, env, client);
        var classes = output2["list"];
        JsonDocument document = JsonDocument.Parse(classes);
        var ids = new List<(string, string)>();
        try
        {
            var property = document.RootElement[0].GetProperty("course");
            for (int i = 0; i < property.GetArrayLength(); i++)
            {
                var course_id = property[i].GetProperty("course_id").GetString();
                var id = property[i].GetProperty("id").GetString();
                if (id is null || course_id is null)
                {
                    throw new System.Exception();
                }
                ids.Add((course_id!, id!));
            }
        }
        catch (System.Exception) { }
        var cookies = output2["cookies"];
        Assert.NotNull(output2["list"]);
        Assert.NotEmpty(output2["list"]);

        var token = WebUtility.UrlDecode(Regex.Match(cookies, "_token=(.*?);").Groups[1].Value);
        token = Regex
            .Match(token, "{i:\\d+;s:\\d+:\"_token\";i:\\d+;s:\\d+:\"(.*?)\";}")
            .Groups[1]
            .Value;

        var spiderGetClassInfo = _spider.ReadCaseFromFile(Config.CasCasePath("smart_class_info"));
        foreach (var (course_id, id) in ids)
        {
            env["course_id"] = course_id;
            env["sub_id"] = id;
            env["token"] = token;
            var output3 = _spider.Run(spiderGetClassInfo, env, client);
            var result = output3["list"];
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
