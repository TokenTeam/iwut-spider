namespace SpiderEngine.Test.ApiTest;

using System.Collections.Generic;
using SpiderEngine.Abstract;
using SpiderEngine.Model;
using SpiderEngine.Service;

public class Redirect
{
    public Redirect()
    {
        _spider = new Spider(new DefaultSpiderHttpClientProvider());
    }

    private readonly ISpider _spider;

    [Theory]
    [InlineData("Tom")]
    [InlineData("Jerry")]
    public void RedirectHelloWithParam(string name)
    {
        var spiderInfo = new SpiderInfo
        {
            Engine = new EngineOptions
            {
                Cookie = true,
                Redirect = true
            },
            Environment = ["baseUrl", "redirectUrl"],
            Task = new[]
            {
                new SpiderTaskInfo
                {
                    Url = "$(baseUrl)/basic/301",
                    Method = SpiderMethod.Get,
                    Payload = new SpiderPayload
                    {
                        Type = SpiderPayloadType.Params,
                        Value = [
                            new SpiderKeyValuePair
                            {
                                Key = "redirectUrl",
                                Value = "$(redirectUrl)"
                            }
                        ]
                    },
                    Content = new SpiderParserInfo
                    {
                        Patten = name,
                        Type = SpiderParserType.Regex,
                        Value = [
                            new SpiderKeyPathPair
                            {
                                Key = "name",
                                Path = "0"
                            }
                        ]
                    }
                }
            },
            Output = ["name"]
        };

        var json = _spider.Serialize(spiderInfo);
        File.WriteAllText($"redirect_hello_param_{name}.json", json);

        var output = _spider.Run(spiderInfo, new Dictionary<string, string>
        {
            ["baseUrl"] = Config.ApiBaseUrl,
            ["redirectUrl"] = $"{Config.ApiBaseUrl}/basic/Hello?name={name}"
        });

        var nameValue = output["name"];
        Assert.Equal(name, nameValue);
    }

    [Theory]
    [InlineData("https://baidu.com")]
    [InlineData("https://google.com")]
    public void RedirectDisabled(string url)
    {
        var spiderInfo = new SpiderInfo
        {
            Engine = new EngineOptions
            {
                Cookie = true,
                Redirect = false
            },
            Environment = ["baseUrl", "redirectUrl"],
            Task = new[]
            {
                new SpiderTaskInfo
                {
                    Url = "$(baseUrl)/basic/301",
                    Method = SpiderMethod.Get,
                    Success = 302,
                    Payload = new SpiderPayload
                    {
                        Type = SpiderPayloadType.Params,
                        Value = [
                            new SpiderKeyValuePair
                            {
                                Key = "redirectUrl",
                                Value = "$(redirectUrl)"
                            }
                        ]
                    },
                    Header = new[]
                    {
                        new SpiderKeyPathPair
                        {
                            Key = "url",
                            Path = "Location"
                        }
                    }
                }
            },
            Output = ["url"]
        };

        var json = _spider.Serialize(spiderInfo);
        File.WriteAllText($"redirect_disabled_{url.Replace("https://", string.Empty)}.json", json);

        var output = _spider.Run(spiderInfo, new Dictionary<string, string>
        {
            ["baseUrl"] = Config.ApiBaseUrl,
            ["redirectUrl"] = url
        });

        var urlValue = output["url"];
        Assert.Equal(url + '/', urlValue);
    }

    [Theory]
    [InlineData("Tom")]
    [InlineData("Jerry")]
    public void RedirectHelloWithUrl(string name)
    {
        var spiderInfo = new SpiderInfo
        {
            Engine = new EngineOptions
            {
                Cookie = true,
                Redirect = true
            },
            Environment = ["baseUrl", "redirectUrl"],
            Task = new[]
            {
                new SpiderTaskInfo
                {
                    Url = "$(baseUrl)/basic/301?redirectUrl=$(redirectUrl)",
                    Method = SpiderMethod.Get,
                    Content = new SpiderParserInfo
                    {
                        Patten = name,
                        Type = SpiderParserType.Regex,
                        Value = [
                            new SpiderKeyPathPair
                            {
                                Key = "name",
                                Path = "0"
                            }
                        ]
                    }
                }
            },
            Output = ["name"]
        };
        var output = _spider.Run(spiderInfo, new Dictionary<string, string>
        {
            ["baseUrl"] = Config.ApiBaseUrl,
            ["redirectUrl"] = $"{Config.ApiBaseUrl}/basic/Hello?name={name}"
        });

        var json = _spider.Serialize(spiderInfo);
        File.WriteAllText($"redirect_hello_{name}.json", json);

        var nameValue = output["name"];
        Assert.Equal(name, nameValue);
    }
}
