using SpiderEngine.Abstract;
using SpiderEngine.Model;
using SpiderEngine.Service;

namespace SpiderEngine.Test.ApiTest;

public class Header
{
    public Header()
    {
        _spider = new Spider(new DefaultSpiderHttpClientProvider());
    }

    private readonly ISpider _spider;

    [Fact]
    public void HeaderEcho()
    {
        var spiderInfo = new SpiderInfo
        {
            Engine = new EngineOptions
            {
                Cookie = true,
            },
            Environment = ["baseUrl"],
            Task = new[]
            {
                new SpiderTaskInfo
                {
                    Url = "$(baseUrl)/header/echo",
                    Method = SpiderMethod.Get,
                    Payload = new()
                    {
                        Type = SpiderPayloadType.Params,
                        Header = [
                            new SpiderKeyValuePair
                            {
                                Key = "Echo",
                                Value = "Tom"
                            }
                        ]
                    },
                    Header = [
                        new SpiderKeyPathPair
                        {
                            Key = "value",
                            Path = "Echo"
                        }
                    ]
                }
            },
            Output = ["value"]
        };

        var json = _spider.Serialize(spiderInfo);
        File.WriteAllText("header_echo.json", json);

        var output = _spider.Run(spiderInfo, new Dictionary<string, string>
        {
            ["baseUrl"] = Config.ApiBaseUrl
        });

        Assert.Equal("Tom", output["value"]);
    }

    [Fact]
    public void HeaderDouble()
    {
        var spiderInfo = new SpiderInfo
        {
            Engine = new EngineOptions
            {
                Cookie = true,
            },
            Environment = ["baseUrl"],
            Task = new[]
            {
                new SpiderTaskInfo
                {
                    Url = "$(baseUrl)/header/double",
                    Method = SpiderMethod.Get,
                    Payload = new()
                    {
                        Type = SpiderPayloadType.Params,
                        Header = [
                            new SpiderKeyValuePair
                            {
                                Key = "Value1",
                                Value = "Tom"
                            },
                            new SpiderKeyValuePair
                            {
                                Key = "Value2",
                                Value = "Jerry"
                            }
                        ]
                    },
                    Header = [
                        new SpiderKeyPathPair
                        {
                            Key = "value",
                            Path = "Value"
                        }
                    ]
                }
            },
            Output = ["value"]
        };

        var json = _spider.Serialize(spiderInfo);
        File.WriteAllText("header_double.json", json);

        var output = _spider.Run(spiderInfo, new Dictionary<string, string>
        {
            ["baseUrl"] = Config.ApiBaseUrl
        });

        Assert.Equal("Tom;Jerry", output["value"]);
    }
}
