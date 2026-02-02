using Microsoft.AspNetCore.Components;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Models;

namespace UI.EmployerPortal.Web.Features.Dashboard.Components;

/// <summary>
/// 
/// </summary>
public partial class EmployerDashboardAccountHeader
{
    [Inject]
    private IDashboardOrchestrator DashboardOrchestrator { get; set; } = default!;

    private Account? _account;

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _account = await DashboardOrchestrator.GetSelectedAccountAsync();
            StateHasChanged();
        }
    }
}
