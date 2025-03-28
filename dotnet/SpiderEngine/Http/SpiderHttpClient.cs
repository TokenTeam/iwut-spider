namespace SpiderEngine.Http;

using System.Text;
using SpiderEngine.Abstract;
using SpiderEngine.Model;

internal class SpiderHttpClient : ISpiderHttpClient
{
    public SpiderHttpClient(EngineOptions engineOptions)
    {
        this.handler = new HttpClientHandler
        {
            AllowAutoRedirect = false,
            UseCookies = engineOptions.Cookie,
            ServerCertificateCustomValidationCallback = engineOptions.ForceSSL ?? true
                ? (message, cert, chain, errors) => true // ignore SSL Check
                : HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
        };

        this.client = new HttpClient(handler);
        this.client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36 Edg/122.0.0.0");
    }

    private readonly HttpClient client;
    private readonly HttpClientHandler handler;

    public async Task<(string Content, IDictionary<string, string> Headers)> GetAsync(string url, IDictionary<string, string> headers, int success = 200, bool redirect = true)
    {
        var message = new HttpRequestMessage(HttpMethod.Get, url);
        foreach (var header in headers)
        {
            message.Headers.Add(header.Key, header.Value);
        }

        var response = await this.client.SendAsync(message);

        while (redirect && response.StatusCode is System.Net.HttpStatusCode.MovedPermanently or System.Net.HttpStatusCode.Redirect)
        {
            response = await this.client.GetAsync(response.Headers.Location);
        }

        if ((int)response.StatusCode != success)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}, expected {success}.");
        }

        var respContent = await response.Content.ReadAsStringAsync();
        var respHeaders = response.Headers.ToDictionary(x => x.Key, x => string.Join(";", x.Value));
        return (respContent, respHeaders);
    }

    public async Task<(string Content, IDictionary<string, string> Headers)> PostAsync(string url, IDictionary<string, string> headers, string payload, SpiderPayloadType type, int success = 200, bool redirect = true)
    {
        var contentType = type switch
        {
            SpiderPayloadType.Json => "application/json",
            SpiderPayloadType.Form => "application/x-www-form-urlencoded",
            _ => "application/text"
        };

        var content = new StringContent(payload, Encoding.UTF8, contentType);

        var message = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content,
        };

        foreach (var header in headers)
        {
            message.Headers.Add(header.Key, header.Value);
        }

        var response = await this.client.SendAsync(message);

        while(redirect && response.StatusCode is System.Net.HttpStatusCode.MovedPermanently or System.Net.HttpStatusCode.Redirect)
        {
            response = await this.client.GetAsync(response.Headers.Location);
        }
        
        if ((int)response.StatusCode != success)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}, expected {success}.");
        }

        var respContent = await response.Content.ReadAsStringAsync();
        var respHeaders = response.Headers.ToDictionary(x => x.Key, x => string.Join(";", x.Value));
        return (respContent, respHeaders);
    }
}
