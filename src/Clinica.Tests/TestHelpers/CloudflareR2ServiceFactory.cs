using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Clinica.Services;
using Microsoft.Extensions.Configuration;

namespace Clinica.Tests.TestHelpers;

internal static class CloudflareR2ServiceFactory
{
    public static CloudflareR2Service Create()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Cloudflare:R2AccessKey"] = "test",
            ["Cloudflare:R2SecretKey"] = "test",
            ["Cloudflare:R2BucketName"] = "test",
            ["Cloudflare:AccountId"] = "test",
            ["Cloudflare:PublicBase"] = "https://example.com"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var httpClientFactory = new FakeHttpClientFactory();
        return new CloudflareR2Service(configuration, httpClientFactory);
    }

    private sealed class FakeHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            return new HttpClient(new FakeHandler())
            {
                BaseAddress = new Uri("https://localhost")
            };
        }
    }

    private sealed class FakeHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
