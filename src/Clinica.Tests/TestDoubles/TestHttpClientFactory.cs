using System.Net.Http;
using Microsoft.Extensions.Http;

namespace Clinica.Tests.TestDoubles;

internal class TestHttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateClient(string name) => new();
}
