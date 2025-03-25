namespace SpiderEngine.Test.ApiTest
{
    using SpiderEngine.Abstract;
    using SpiderEngine.Model;
    using SpiderEngine.Service;

    public class Basic
    {
        public Basic()
        {
            _spider = new Spider(new DefaultSpiderHttpClientProvider());
        }

        private readonly ISpider _spider;

        [Fact]
        public void GetSchema()
        {
            var schema = _spider.GetSchema();
            File.WriteAllText("schema.json", schema);
            Assert.NotNull(schema);
        }

        [Fact]
        public void EnvironmentAndOutput()
        {
            var spiderInfo = new SpiderInfo
            {
                Engine = new EngineOptions
                {
                    Cookie = true,
                    Redirect = true
                },
                Environment = ["a", "b", "c"],
                Output = ["a", "b", "c"],
            };

            var json = _spider.Serialize(spiderInfo);
            File.WriteAllText("environment_and_output.json", json);

            var output = _spider.Run(spiderInfo, new Dictionary<string, string>
            {
                ["a"] = "1",
                ["b"] = "2",
                ["c"] = "3"
            });

            Assert.Equal("1", output["a"]);
            Assert.Equal("2", output["b"]);
            Assert.Equal("3", output["c"]); 
        }

        [Theory]
        [InlineData(200)]
        [InlineData(404)]
        [InlineData(500)]
        public void ResponseCode(int code)
        {
            var spiderInfo = new SpiderInfo
            {
                Engine = new EngineOptions
                {
                    Cookie = true,
                    Redirect = true,
                    ForceSSL = true
                },
                Environment = ["baseUrl", "code"],
                Task = new[]
                {
                    new SpiderTaskInfo
                    {
                        Url = "$(baseUrl)/basic/$(code)",
                        Method = SpiderMethod.Get,
                        Success = code
                    }
                }
            };
            _spider.Run(spiderInfo, new Dictionary<string, string>
            {
                ["baseUrl"] = Config.ApiBaseUrl,
                ["code"] = code.ToString()
            });

            var json = _spider.Serialize(spiderInfo);
            File.WriteAllText($"response_code_{code}.json", json);

            Assert.True(true);
        }

        [Theory]
        [InlineData("Tom")]
        [InlineData("Jerry")]
        public void Hello(string name)
        {
            var spiderInfo = new SpiderInfo
            {
                Engine = new EngineOptions
                {
                    Cookie = true,
                    Redirect = true
                },
                Environment = ["baseUrl", "name"],
                Task = new[]
                {
                    new SpiderTaskInfo
                    {
                        Url = "$(baseUrl)/basic/Hello?name=$(name)",
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

            var json = _spider.Serialize(spiderInfo);
            File.WriteAllText($"hello_{name}.json", json);

            var output = _spider.Run(spiderInfo, new Dictionary<string, string>
            {
                ["baseUrl"] = Config.ApiBaseUrl,
                ["name"] = name
            });

            var nameValue = output["name"];
            Assert.Equal(name, nameValue);
        }
    }
}
