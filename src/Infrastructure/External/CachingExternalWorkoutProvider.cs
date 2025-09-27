using Application.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Infrastructure.External;

public class CachingExternalWorkoutProvider : IExternalWorkoutProvider
{
    private readonly IExternalWorkoutProvider _inner;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingExternalWorkoutProvider> _logger;
    private readonly TimeSpan _listTtl;
    private readonly TimeSpan _itemTtl;

    public CachingExternalWorkoutProvider(IExternalWorkoutProvider inner, IMemoryCache cache, ILogger<CachingExternalWorkoutProvider> logger, ExternalWorkoutCacheOptions opts)
    {
        _inner = inner;
        _cache = cache;
        _logger = logger;
        _listTtl = TimeSpan.FromSeconds(Math.Max(5, opts.ListTtlSeconds));
        _itemTtl = TimeSpan.FromSeconds(Math.Max(10, opts.ItemTtlSeconds));
    }

    public async Task<IReadOnlyCollection<ExternalWorkoutDto>> GetWorkoutsAsync(int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        string key = $"ext_workouts_{page}_{pageSize}";
        if (_cache.TryGetValue(key, out IReadOnlyCollection<ExternalWorkoutDto>? cached) && cached != null)
        {
            _logger.LogDebug("Cache hit for workouts page={Page} size={Size}", page, pageSize);
            return cached;
        }
        var data = await _inner.GetWorkoutsAsync(page, pageSize, ct);
        _cache.Set(key, data, _listTtl);
        return data;
    }

    public async Task<ExternalWorkoutDto?> GetWorkoutByIdAsync(string externalId, CancellationToken ct = default)
    {
        string key = $"ext_workout_{externalId}";
        if (_cache.TryGetValue(key, out ExternalWorkoutDto? cached) && cached != null)
        {
            _logger.LogDebug("Cache hit for workout {ExternalId}", externalId);
            return cached;
        }
        var item = await _inner.GetWorkoutByIdAsync(externalId, ct);
        if (item != null)
            _cache.Set(key, item, _itemTtl);
        return item;
    }
}