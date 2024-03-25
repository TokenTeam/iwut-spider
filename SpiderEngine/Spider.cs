namespace SpiderEngine
{
    using System.Collections.Generic;
    using SpiderEngine.Abstract;
    using SpiderEngine.Model;
    using SpiderEngine.Utility;

    internal class Spider : ISpider
    {
        public Spider(ISpiderHttpClientProvider httpClientProvider)
        {
            this.httpClientProvider = httpClientProvider;
        }

        private readonly ISpiderHttpClientProvider httpClientProvider;

        public IDictionary<string, string> Run(SpiderInfo spiderInfo, IDictionary<string, string> environment)
        {
            // check environment
            ArgumentNullException.ThrowIfNull(environment);
            if (!spiderInfo.Environment.All(environment.ContainsKey))
            {
                throw new ArgumentException("Missing environment variables.", nameof(environment));
            }

            // create http client
            var httpClient = httpClientProvider.Create(spiderInfo.Engine);

            // run steps
            foreach (var spiderTask in spiderInfo.Task)
            {
                RunStep(spiderTask, environment, httpClient).Wait();
            }

            // check output
            if (!spiderInfo.Output.All(environment.ContainsKey))
            {
                throw new InvalidOperationException("Missing output variables.");
            }

            return spiderInfo.Output.ToDictionary(key => key, key => environment[key]);
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
            var successCode = spiderTask.Success == 0 ? 200 : spiderTask.Success;
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
                (respContent, respHeaders) = await httpClient.GetAsync(url.FillVariables(environment), requestHeaders, successCode);
            }
            else
            {
                (respContent, respHeaders) = await httpClient.PostAsync(spiderTask.Url.FillVariables(environment), requestHeaders, payload, payloadInfo?.Type ?? SpiderPayloadType.Text);
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
}
