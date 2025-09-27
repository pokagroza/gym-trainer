using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Abstractions;
using Infrastructure.External;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Application.Tests.External;

public class HttpExternalWorkoutProviderTests
{
    private class MockHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;
        public int CallCount { get; private set; }
        public MockHandler(Func<HttpRequestMessage, HttpResponseMessage> handler) => _handler = handler;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.FromResult(_handler(request));
        }
    }

    private static (HttpExternalWorkoutProvider provider, MockHandler handler) Create(object? payload)
    {
        var handler = new MockHandler(req =>
        {
            if (payload == null)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
        });
        var http = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var options = Options.Create(new ExternalWorkoutApiOptions { BaseUrl = "https://example.test", TimeoutSeconds = 5 });
        var provider = new HttpExternalWorkoutProvider(http, options, NullLogger<HttpExternalWorkoutProvider>.Instance);
        return (provider, handler);
    }

    [Fact]
    public async Task GetWorkoutsAsync_Returns_Data()
    {
        var sample = new []
        {
            new ExternalWorkoutDto("id1", "Title1", null, Array.Empty<ExternalExerciseDto>()),
            new ExternalWorkoutDto("id2", "Title2", null, Array.Empty<ExternalExerciseDto>())
        };
        var (provider, handler) = Create(sample);
        var result = await provider.GetWorkoutsAsync(2, 5); // page=2 size=5
        Assert.Equal(2, result.Count);
        Assert.True(handler.CallCount == 1);
    }

    [Fact]
    public async Task GetWorkoutByIdAsync_Returns_Null_On_Error()
    {
        var (provider, handler) = Create(null);
        var res = await provider.GetWorkoutByIdAsync("abc");
        Assert.Null(res);
    }
}
