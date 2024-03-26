using SpiderEngine.Abstract;
using SpiderEngine.Model;
using SpiderEngine.Service;

namespace SpiderEngine.Test.ApiTest;

public class Post
{
    public Post()
    {
        _spider = new Spider(new DefaultSpiderHttpClientProvider());
    }

    private readonly ISpider _spider;

    [Fact]
    public void PostForm()
    {
        var spiderInfo = new SpiderInfo
        {
            Engine = new EngineOptions
            {
                Cookie = true,
                Redirect = true
            },
            Environment = ["baseUrl"],
            Task = new[]
            {
                new SpiderTaskInfo
                {
                    Url = "$(baseUrl)/form/user",
                    Method = SpiderMethod.Post,
                    Payload = new SpiderPayload
                    {
                        Type = SpiderPayloadType.Form,
                        Value = [
                            new SpiderKeyValuePair
                            {
                                Key = "name",
                                Value = "Tom"
                            },
                            new SpiderKeyValuePair
                            {
                                Key = "age",
                                Value = "18"
                            }
                        ]
                    },
                    Content = new SpiderParserInfo
                    {
                        Type = SpiderParserType.Json,
                        Value = [
                            new SpiderKeyPathPair
                            {
                                Key = "a",
                                Path = "name"
                            },
                            new SpiderKeyPathPair
                            {
                                Key = "b",
                                Path = "age"
                            }
                        ]
                    }
                }
            },
            Output = ["a", "b"]
        };

        var output = _spider.Run(spiderInfo, new Dictionary<string, string>
        {
            { "baseUrl", Config.ApiBaseUrl }
        });
        Assert.Equal("Tom", output["a"]);
        Assert.Equal("18", output["b"]);
    }

    [Fact]
    public void PostJson()
    {
        var spiderInfo = new SpiderInfo
        {
            Engine = new EngineOptions
            {
                Cookie = true,
                Redirect = true
            },
            Environment = ["baseUrl"],
            Task = new[]
            {
                new SpiderTaskInfo
                {
                    Url = "$(baseUrl)/json/user",
                    Method = SpiderMethod.Post,
                    Payload = new SpiderPayload
                    {
                        Type = SpiderPayloadType.Json,
                        Value = [
                            new SpiderKeyValuePair
                            {
                                Key = "name",
                                Value = "Jerry",
                                Type = SpiderValueType.String
                            },
                            new SpiderKeyValuePair
                            {
                                Key = "age",
                                Value = "20",
                                Type = SpiderValueType.Number
                            }
                        ]
                    },
                    Content = new SpiderParserInfo
                    {
                        Type = SpiderParserType.Json,
                        Value = [
                            new SpiderKeyPathPair
                            {
                                Key = "a",
                                Path = "name"
                            },
                            new SpiderKeyPathPair
                            {
                                Key = "b",
                                Path = "age"
                            }
                        ]
                    }
                }
            },
            Output = ["a", "b"]
        };

        var output = _spider.Run(spiderInfo, new Dictionary<string, string>
        {
            { "baseUrl", Config.ApiBaseUrl }
        });
        Assert.Equal("Jerry", output["a"]);
        Assert.Equal("20", output["b"]);
    }

    [Fact]
    public void ParseJson()
    {
        var spiderInfo = new SpiderInfo
        {
            Engine = new EngineOptions
            {
                Cookie = true,
                Redirect = true
            },
            Environment = ["baseUrl"],
            Task = new[]
            {
                new SpiderTaskInfo
                {
                    Url = "$(baseUrl)/json/user",
                    Method = SpiderMethod.Get,
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

        var output = _spider.Run(spiderInfo, new Dictionary<string, string>
        {
            { "baseUrl", Config.ApiBaseUrl }
        });
        Assert.Equal("Alice", output["name_0"]);
        Assert.Equal("20", output["age_0"]);
        Assert.Equal("Bob", output["name_1"]);
        Assert.Equal("30", output["age_1"]);
        Assert.Equal("Charlie", output["name_2"]);
        Assert.Equal("40", output["age_2"]);
    }
}
