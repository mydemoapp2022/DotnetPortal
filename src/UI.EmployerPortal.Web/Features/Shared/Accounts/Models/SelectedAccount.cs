
using UI.EmployerPortal.Web.Features.Shared.Session.Models;

namespace UI.EmployerPortal.Web.Features.Shared.Accounts.Models;

/// <summary>
/// Session Model for storing the currently selected employer account.
/// Used to persist the selected account across page navigations.
/// </summary>
public sealed record SelectedAccount : ISessionModel
{
    /// <summary>
    /// Gets or sets the selected employer account. 
    /// </summary>
    public Account? Account { get; init; }
}
