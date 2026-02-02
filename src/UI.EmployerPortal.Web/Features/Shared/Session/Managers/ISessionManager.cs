using UI.EmployerPortal.Web.Features.Shared.Session.Models;

namespace UI.EmployerPortal.Web.Features.Shared.Session.Managers;

/// <summary>
/// Interface for session management operations.
/// Implementations: ProtectedSessionManager, DistributedSessionManager
/// </summary>
public interface ISessionManager
{
    /// <summary>
    /// Retrieves a session model of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of session model to retrieve. Must implement <see cref="ISessionModel"/>.</typeparam>
    /// <returns>The session model if found; otherwise null.</returns>
    Task<T?> GetAsync<T>() where T : ISessionModel;

    /// <summary>
    /// Stores a session model in the session container.
    /// </summary>
    /// <typeparam name="T">The type of session model to store. Must implement <see cref="ISessionModel"/>.</typeparam>
    /// <param name="model">The session model to store.</param>
    Task SetAsync<T>(T model) where T : ISessionModel;

    /// <summary>
    /// Removes a session model of the specified type from the session storage.
    /// </summary>
    /// <typeparam name="T">The type of session model to remove. Must implent <see cref="ISessionModel"/>.</typeparam>
    Task ClearAsync<T>() where T : ISessionModel;
}
