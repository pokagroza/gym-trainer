using Application.Abstractions;
using Infrastructure.External;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Application.Tests.External;

public class CachingExternalWorkoutProviderTests
{
    private class FakeProvider : IExternalWorkoutProvider
    {
        public int ListCalls { get; private set; }
        public int ItemCalls { get; private set; }
        public Task<IReadOnlyCollection<ExternalWorkoutDto>> GetWorkoutsAsync(int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            ListCalls++;
            IReadOnlyCollection<ExternalWorkoutDto> data = new [] { new ExternalWorkoutDto("e1","W1",null, Array.Empty<ExternalExerciseDto>()) };
            return Task.FromResult(data);
        }
        public Task<ExternalWorkoutDto?> GetWorkoutByIdAsync(string externalId, CancellationToken ct = default)
        {
            ItemCalls++;
            return Task.FromResult<ExternalWorkoutDto?>(new ExternalWorkoutDto(externalId, "Title", null, Array.Empty<ExternalExerciseDto>()));
        }
    }

    [Fact]
    public async Task List_Is_Cached_By_Page_And_Size()
    {
        var inner = new FakeProvider();
        var cache = new MemoryCache(new MemoryCacheOptions());
    var provider = new CachingExternalWorkoutProvider(inner, cache, NullLogger<CachingExternalWorkoutProvider>.Instance, new ExternalWorkoutCacheOptions { ListTtlSeconds = 60, ItemTtlSeconds = 120 });

        var r1 = await provider.GetWorkoutsAsync(1, 5);
        var r2 = await provider.GetWorkoutsAsync(1, 5);
        var r3 = await provider.GetWorkoutsAsync(2, 5); // different page => new call
        Assert.Equal(2, inner.ListCalls); // page1 once + page2 once
    }

    [Fact]
    public async Task Item_Is_Cached()
    {
        var inner = new FakeProvider();
        var cache = new MemoryCache(new MemoryCacheOptions());
    var provider = new CachingExternalWorkoutProvider(inner, cache, NullLogger<CachingExternalWorkoutProvider>.Instance, new ExternalWorkoutCacheOptions());
        var a = await provider.GetWorkoutByIdAsync("x1");
        var b = await provider.GetWorkoutByIdAsync("x1");
        Assert.Equal(1, inner.ItemCalls);
    }
}
