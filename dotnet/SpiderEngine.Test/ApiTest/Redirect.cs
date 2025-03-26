namespace SpiderEngine.Test.ApiTest;

using System.Collections.Generic;
using System.Text.Encodings.Web;
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

    [Fact]
    public void RedirectPost()
    {
        var redirectSpiderInfo = new SpiderInfo
        {
            Engine = new EngineOptions
            {
                Cookie = true,
                Redirect = true,
                ForceSSL = false,
            },
            Environment = ["baseUrl"],
            Task = new[]
            {
                new SpiderTaskInfo
                {
                    Url = "$(baseUrl)",
                    Method = SpiderMethod.Post,
                    Success = 200,
                    Content = new SpiderParserInfo
                    {
                        Type = SpiderParserType.Json,
                        Value = [
                            new SpiderKeyPathPair
                            {
                                Key = "name_0",
                                Path = "0.name"
                            },
                            new SpiderKeyPathPair
                            {
                                Key = "age_0",
                                Path = "0.age"
                            },
                            new SpiderKeyPathPair
                            {
                                Key = "name_1",
                                Path = "1.name"
                            },
                            new SpiderKeyPathPair
                            {
                                Key = "age_1",
                                Path = "1.age"
                            },
                            new SpiderKeyPathPair
                            {
                                Key = "name_2",
                                Path = "2.name"
                            },
                            new SpiderKeyPathPair
                            {
                                Key = "age_2",
                                Path = "2.age"
                            }
                        ]
                    }
                }
            },
            Output = ["name_0", "age_0", "name_1", "age_1", "name_2", "age_2"]
        };
        var nonRedirectSpiderInfo = new SpiderInfo
        {
            Engine = new EngineOptions
            {
                Cookie = true,
                Redirect = false,
                ForceSSL = false,
            },
            Environment = ["baseUrl"],
            Task = new[]
            {
                new SpiderTaskInfo
                {
                    Url = "$(baseUrl)",
                    Method = SpiderMethod.Post,
                    Success = 302,
                    Content = null
                }
            },
            Output = []
        };
        var json = _spider.Serialize(redirectSpiderInfo);
        File.WriteAllText("parse_json.json", json);

        var url = $"{Config.ApiBaseUrl}/json/redirectTest?redirectUrl={UrlEncoder.Default.Encode(Config.ApiBaseUrl + "/Json/user")}";
        var output = _spider.Run(redirectSpiderInfo, new Dictionary<string, string>
        {
            { "baseUrl", url }
        });

        Assert.Equal("Alice", output["name_0"]);
        Assert.Equal("20", output["age_0"]);
        Assert.Equal("Bob", output["name_1"]);
        Assert.Equal("30", output["age_1"]);
        Assert.Equal("Charlie", output["name_2"]);
        Assert.Equal("40", output["age_2"]);

        json = _spider.Serialize(nonRedirectSpiderInfo);
        File.WriteAllText("parse_json.json", json);
        output = _spider.Run(nonRedirectSpiderInfo, new Dictionary<string, string>
        {
            { "baseUrl", url }
        });
    }

    [Fact]
    public void MovePermanentlyPost()
    {
        var redirectSpiderInfo = new SpiderInfo
        {
            Engine = new EngineOptions
            {
                Cookie = true,
                Redirect = true,
                ForceSSL = false,
            },
            Environment = ["baseUrl"],
            Task = new[]
            {
                new SpiderTaskInfo
                {
                    Url = "$(baseUrl)",
                    Method = SpiderMethod.Post,
                    Success = 200,
                    Content = new SpiderParserInfo
                    {
                        Type = SpiderParserType.Json,
                        Value = [
                            new SpiderKeyPathPair
                            {
                                Key = "name_0",
                                Path = "0.name"
                            },
                            new SpiderKeyPathPair
                            {
                                Key = "age_0",
                                Path = "0.age"
                            },
                            new SpiderKeyPathPair
                            {
                                Key = "name_1",
                                Path = "1.name"
                            },
                            new SpiderKeyPathPair
                            {
                                Key = "age_1",
                                Path = "1.age"
                            },
                            new SpiderKeyPathPair
                            {
                                Key = "name_2",
                                Path = "2.name"
                            },
                            new SpiderKeyPathPair
                            {
                                Key = "age_2",
                                Path = "2.age"
                            }
                        ]
                    }
                }
            },
            Output = ["name_0", "age_0", "name_1", "age_1", "name_2", "age_2"]
        };
        var nonRedirectSpiderInfo = new SpiderInfo
        {
            Engine = new EngineOptions
            {
                Cookie = true,
                Redirect = false,
                ForceSSL = false,
            },
            Environment = ["baseUrl"],
            Task = new[]
            {
                new SpiderTaskInfo
                {
                    Url = "$(baseUrl)",
                    Method = SpiderMethod.Post,
                    Success = 301,
                    Content = null
                }
            },
            Output = []
        };
        var json = _spider.Serialize(redirectSpiderInfo);
        File.WriteAllText("parse_json.json", json);
        var url = $"{Config.ApiBaseUrl}/json/movePermanently?redirectUrl={UrlEncoder.Default.Encode(Config.ApiBaseUrl + "/Json/user")}";
        var output = _spider.Run(redirectSpiderInfo, new Dictionary<string, string>
        {
            { "baseUrl", url },
        });

        Assert.Equal("Alice", output["name_0"]);
        Assert.Equal("20", output["age_0"]);
        Assert.Equal("Bob", output["name_1"]);
        Assert.Equal("30", output["age_1"]);
        Assert.Equal("Charlie", output["name_2"]);
        Assert.Equal("40", output["age_2"]);

        json = _spider.Serialize(nonRedirectSpiderInfo);
        File.WriteAllText("parse_json.json", json);
        output = _spider.Run(nonRedirectSpiderInfo, new Dictionary<string, string>
        {
            { "baseUrl", url },
        });
    }
}
