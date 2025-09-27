using System.Net.Http.Json;
using Application.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Infrastructure.External;

public class HttpExternalWorkoutProvider : IExternalWorkoutProvider
{
    private readonly HttpClient _http;
    private readonly ExternalWorkoutApiOptions _options;
    private readonly ILogger<HttpExternalWorkoutProvider> _logger;

    public HttpExternalWorkoutProvider(HttpClient http, IOptions<ExternalWorkoutApiOptions> options, ILogger<HttpExternalWorkoutProvider> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
        if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
            _http.BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/");
        _http.Timeout = TimeSpan.FromSeconds(Math.Max(3, _options.TimeoutSeconds));
        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
            _http.DefaultRequestHeaders.Add("X-API-Key", _options.ApiKey);
    }

    public async Task<IReadOnlyCollection<ExternalWorkoutDto>> GetWorkoutsAsync(int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 50) pageSize = 50;
        var url = $"workouts?page={page}&pageSize={pageSize}"; // depends on external API contract
        try
        {
            var data = await _http.GetFromJsonAsync<List<ExternalWorkoutDto>>(url, ct) ?? new();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch workouts from external API");
            return Array.Empty<ExternalWorkoutDto>();
        }
    }

    public async Task<ExternalWorkoutDto?> GetWorkoutByIdAsync(string externalId, CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<ExternalWorkoutDto>($"workouts/{externalId}", ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch workout {ExternalId}", externalId);
            return null;
        }
    }
}