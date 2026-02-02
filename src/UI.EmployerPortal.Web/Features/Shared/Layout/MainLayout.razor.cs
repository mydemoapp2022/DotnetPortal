using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace UI.EmployerPortal.Web.Features.Shared.Layout;

/// <summary>
/// MainLayout
/// </summary>
public partial class MainLayout : IDisposable
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private bool ShowSideMenu
    {
        get
        {
            var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri).ToLower();

            // Pages where menu should NOT be shown
            var excludedPaths = new[] { "", "employer-dashboard" };

            return !excludedPaths.Contains(relativePath.Split('?')[0].TrimEnd('/'));
        }
    }

    /// <summary>
    /// OnInitialized
    /// </summary>
    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        StateHasChanged();
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }
}
