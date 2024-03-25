using SpiderEngine.Model;
using SpiderEngine.Service;

//var spiderInfo = new SpiderInfo
//{
//    Engine = new EngineOptions
//    {
//        Cookie = true,
//        Redirect = false
//    },
//    Environment = [],
//    Name = "Spider",
//    Version = 1,
//    Task = new[]
//    {
//        new SpiderTaskInfo
//        {
//            Name = "Task1",
//            Url = "https://zhlgd.whut.edu.cn/tpass/rsa?skipWechat=$(skip_wechat)",
//            Method = SpiderMethod.Post,
//            Content = new SpiderParserInfo
//            {
//                Type = SpiderParserType.Json,
//                Value = new[]
//                {
//                    new SpiderKeyPathPair
//                    {
//                        Key = "key",
//                        Path = "publicKey"
//                    }
//                }
//            }
//        }
//    },
//    Output = new[] { "key" }
//};

var spiderInfo = new SpiderInfo
{
    Engine = new EngineOptions
    {
        Cookie = true,
        Redirect = false
    },
    Environment = ["ias_redirect_url"],
    Name = "Spider",
    Version = 1,
    Task = new[]
    {
        new SpiderTaskInfo
        {
            Name = "Task1",
            Url = "https://zhlgd.whut.edu.cn/tpass/login",
            Success = 200,
            Payload = new SpiderPayload
            {
                Type = SpiderPayloadType.Params,
                Value = new[]
                {
                    new SpiderKeyValuePair
                    {
                        Key = "service",
                        Value = "$(ias_redirect_url)"
                    }
                }
            },
            Method = SpiderMethod.Get,
            Content = new SpiderParserInfo
            {
                Type = SpiderParserType.Regex,
                Patten = @"(LT-\d+-.+?-tpass)",
                Value = new[]
                {
                    new SpiderKeyPathPair
                    {
                        Key = "lt",
                        Path = "1"
                    }
                }
            },
            Header = new[]
            {
                new SpiderKeyPathPair
                {
                    Key = "cookie",
                    Path = "Set-Cookie"
                }
            }
        }
    },
    Output = ["lt", "cookie"]
};

var httpClientProvider = new DefaultSpiderHttpClientProvider();
var spider = new SpiderEngine.Spider(httpClientProvider);

var env = new Dictionary<string, string>
{
    { "ias_redirect_url", "https://zhlgd.whut.edu.cn/tp_up/" }
};

var result = spider.Run(spiderInfo, env);

Console.ReadLine();