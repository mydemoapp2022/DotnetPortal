using UI.EmployerPortal.Web.Features.Shared.Accounts.Models;
using UI.EmployerPortal.Web.Features.Shared.Session.Managers;

namespace UI.EmployerPortal.Web.Features.Landing;

/// <summary>
/// Interface for landing orchestration operations
/// Manages account selection and session storage.
/// </summary>
public interface ILandingOrchestrator
{
    /// <summary>
    /// Stores the selected account in session storage and repares for navigation. 
    /// </summary>    
    /// <param name="account">The acccount selected by the user.</param>
    /// <returns></returns>
    Task SelectAccountAsync(Account account);
}

/// <summary>
/// Orchestrator for the landing page.
/// Handle account selection and session management.
/// </summary>
public class LandingOrchestrator : ILandingOrchestrator
{
    private readonly ISessionManager _sessionManager;

    /// <summary>
    /// Initialize a new instance of the <see cref="LandingOrchestrator"/> class. 
    /// </summary>
    /// <param name="sessionManager">The session manager for storing session data.</param>
    public LandingOrchestrator(ISessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    ///<inheritdoc />
    public async Task SelectAccountAsync(Account account)
    {
        await _sessionManager.SetAsync(new SelectedAccount { Account = account });
    }
}
