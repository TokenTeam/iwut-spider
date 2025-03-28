namespace SpiderEngine;

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using SpiderEngine.Abstract;
using SpiderEngine.Model;
using SpiderEngine.Utility;

internal class Spider : ISpider
{
    public Spider(ISpiderHttpClientProvider httpClientProvider)
    {
        this.httpClientProvider = httpClientProvider;
        this.jsonOption = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) },
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
        };
    }

    private readonly ISpiderHttpClientProvider httpClientProvider;
    private readonly JsonSerializerOptions jsonOption;

    public ISpiderHttpClient CreateClient(SpiderInfo spiderInfo)
    {        
        // create http client
        var httpClient = httpClientProvider.Create(spiderInfo.Engine);
        return httpClient;
    }

    public IDictionary<string, string> Run(SpiderInfo spiderInfo, IDictionary<string, string> environment, ISpiderHttpClient? httpClient = null)
    {
        // check environment
        ArgumentNullException.ThrowIfNull(environment);

        var missingKeys = spiderInfo.Environment?.Except(environment.Keys ?? Enumerable.Empty<string>()).ToList();
        if (missingKeys?.Count > 0)
        {
            throw new InvalidOperationException($"Missing environment variables: {string.Join(", ", missingKeys)}.");
        }

        httpClient ??= CreateClient(spiderInfo);

        // run steps
        foreach (var spiderTask in spiderInfo.Task ?? Array.Empty<SpiderTaskInfo>())
        {
            try
            {
                RunStep(spiderTask, environment, httpClient).Wait();
                if (spiderTask.Delay != 0)
                {
                    Thread.Sleep(spiderTask.Delay);
                }
            }
            catch (System.Exception ex)
            {
                throw new AggregateException($"Run step {spiderTask.Name} failed.", ex);
            }
        }

        // check output
        if (spiderInfo.Output == null)
        {
            return new Dictionary<string, string>();
        }

        if (!spiderInfo.Output.All(environment.ContainsKey))
        {
            throw new InvalidOperationException("Missing output variables.");
        }

        return spiderInfo.Output.ToDictionary(key => key, key => environment[key]);
    }

    public SpiderInfo Deserialize(string json)
    {
        return JsonSerializer.Deserialize<SpiderInfo>(json, jsonOption) 
            ?? throw new InvalidOperationException("Deserialize spiderInfo failed.");
    }

    public string Serialize(SpiderInfo spiderInfo)
    {
        return JsonSerializer.Serialize(spiderInfo, jsonOption);
    }

    public string GetSchema()
    {
        return SpiderSchema.GetSchema();
    }

    private static async Task RunStep(SpiderTaskInfo spiderTask, IDictionary<string, string> environment, ISpiderHttpClient httpClient)
    {
        // parse payload
        var payloadInfo = spiderTask.Payload;
        string payload = string.Empty;
        if (payloadInfo != null)
        {
            var payloadParser = IPayloadParser.Create(payloadInfo.Type);
            payloadParser.Context = environment;
            payload = payloadParser.Parse(payloadInfo.Patten, payloadInfo.Value);
        }

        // send request
        var successCode = spiderTask.Success == 0 ? 200 : spiderTask.Success ?? 200;
        IDictionary<string, string> respHeaders;
        string respContent;

        var requestHeaders = new Dictionary<string, string>();
        foreach (var pair in payloadInfo?.Header ?? Array.Empty<SpiderKeyValuePair>())
        {
            requestHeaders[pair.Key] = pair.Value.FillVariables(environment);
        }

        if (spiderTask.Method == SpiderMethod.Get)
        {
            var url = spiderTask.Url + (string.IsNullOrEmpty(payload) ? string.Empty : $"?{payload}");
            (respContent, respHeaders) = await httpClient.GetAsync(url.FillVariables(environment), requestHeaders, successCode, spiderTask.Redirect ?? true);
        }
        else
        {
            (respContent, respHeaders) = await httpClient.PostAsync(spiderTask.Url.FillVariables(environment), requestHeaders, payload, payloadInfo?.Type ?? SpiderPayloadType.Text, successCode, spiderTask.Redirect ?? true);
        }

        // parse content
        var contentParserInfo = spiderTask.Content;
        if (contentParserInfo != null)
        {
            var contentParser = IContentParser.Create(contentParserInfo.Type);
            contentParser.Context = environment;
            contentParser.Parse(respContent, contentParserInfo.Patten, contentParserInfo.Value);
        }

        // save header
        foreach (var pair in spiderTask.Header ?? Array.Empty<SpiderKeyPathPair>())
        {
            var success = respHeaders.TryGetValue(pair.Path, out var value);
            if (!success)
            {
                throw new InvalidOperationException($"Header {pair.Path} not found.");
            }
            environment[pair.Key] = value ?? string.Empty;
        }
    }
}
