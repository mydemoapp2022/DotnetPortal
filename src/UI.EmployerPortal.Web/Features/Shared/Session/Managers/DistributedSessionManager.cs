using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using UI.EmployerPortal.Web.Features.Shared.Session.Models;

namespace UI.EmployerPortal.Web.Features.Shared.Session.Managers;

/// <summary>
/// Session manager implementation using SQL Server distributed cache.
/// Data is stored in SQL Server and can be shared across server instances.
/// Requires a session ID to be set before use.
/// </summary>
public class DistributedSessionManager : ISessionManager
{
    private readonly IDistributedCache _cache;
    private readonly ISessionIdProvider _sessionIdProvider;

    private static readonly DistributedCacheEntryOptions DefaultCacheOptions = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(30),
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
    };

    /// <summary>
    /// Initialize a new instance of the <see cref="DistributedSessionManager" /> class.
    /// </summary>
    /// <param name="cache">The distributed cache service.</param>
    /// <param name="sessionIdProvider">The session Id provider for generating unique cache keys.</param>
    public DistributedSessionManager(IDistributedCache cache, ISessionIdProvider sessionIdProvider)
    {
        _cache = cache;
        _sessionIdProvider = sessionIdProvider;
    }

    private string GetKey<T>() where T : ISessionModel
    {
        var sessionId = _sessionIdProvider.GetSessionId();
        return string.IsNullOrEmpty(sessionId)
            ? throw new InvalidOperationException("Session ID is not available.")
            : $"{sessionId}:{typeof(T).Name}";
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>() where T : ISessionModel
    {
        var key = GetKey<T>();
        var data = await _cache.GetStringAsync(key);

        return string.IsNullOrEmpty(data) ? default : JsonSerializer.Deserialize<T>(data);
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(T model) where T : ISessionModel
    {
        var key = GetKey<T>();
        var data = JsonSerializer.Serialize(model);
        await _cache.SetStringAsync(key, data, DefaultCacheOptions);
    }

    /// <inheritdoc />
    public async Task ClearAsync<T>() where T : ISessionModel
    {
        var key = GetKey<T>();
        await _cache.RemoveAsync(key);
    }
}
