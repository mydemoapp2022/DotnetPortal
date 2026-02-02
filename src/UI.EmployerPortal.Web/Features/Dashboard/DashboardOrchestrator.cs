using UI.EmployerPortal.Web.Features.Shared.Accounts.Models;
using UI.EmployerPortal.Web.Features.Shared.Session.Managers;

namespace UI.EmployerPortal.Web.Features.Dashboard;

/// <summary>
/// Interface for dashboard orchestration operations
/// Manages session data access for employer dashboard.
/// </summary>
public interface IDashboardOrchestrator
{
    /// <summary>
    /// Retrieves the currently selected employer account from session storage. 
    /// </summary>
    /// <returns>The selected account if available; otherwise null.</returns>
    Task<Account?> GetSelectedAccountAsync();
}

/// <summary>
/// Orchestrator for the employer dashbaord.
/// Handle session management and data retrieval for dashboard components.
/// </summary>
public class DashboardOrchestrator : IDashboardOrchestrator
{
    private readonly ISessionManager _sessionManager;

    /// <summary>
    /// Initialize a new instance of the <see cref="DashboardOrchestrator"/> class.
    /// </summary>
    /// <param name="sessionManager">The session manager for accessing session data.</param>
    public DashboardOrchestrator(ISessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    ///<inheritdoc />
    public async Task<Account?> GetSelectedAccountAsync()
    {
        var selectedAccount = await _sessionManager.GetAsync<SelectedAccount>();
        return selectedAccount?.Account;
    }
}
