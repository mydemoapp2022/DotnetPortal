using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using UI.EmployerPortal.Web.Features.Shared.Session.Models;

namespace UI.EmployerPortal.Web.Features.Shared.Session.Managers;

/// <summary>
/// Session manager implementation using browser's ProtectedSessionStorage.
/// Data is stored in browser sessionStorage and encrypted using Data Protection API.
/// </summary>
public class ProtectedSessionManager : ISessionManager
{
    private readonly ProtectedSessionStorage _sessionStorage;

    /// <summary>
    /// Initialize a new instance of the <see cref="ProtectedSessionManager"/> class.
    /// </summary>
    /// <param name="sessionStorage">The protected session storage service.</param>
    public ProtectedSessionManager(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>() where T : ISessionModel
    {
        var result = await _sessionStorage.GetAsync<T>(typeof(T).Name);
        return result.Success ? result.Value : default;
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(T model) where T : ISessionModel
    {
        await _sessionStorage.SetAsync(typeof(T).Name, model);
    }

    /// <inheritdoc />
    public async Task ClearAsync<T>() where T : ISessionModel
    {
        await _sessionStorage.DeleteAsync(typeof(T).Name);
    }
}
