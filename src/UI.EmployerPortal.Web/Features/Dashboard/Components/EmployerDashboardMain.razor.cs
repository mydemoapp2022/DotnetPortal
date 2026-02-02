namespace UI.EmployerPortal.Web.Features.Dashboard.Components;

/// <summary>
/// EmployerDashboardMain
/// </summary>
public partial class EmployerDashboardMain
{
    private bool _isListView = true;

    private void SetView(bool isListView)
    {
        _isListView = isListView;
    }
}
