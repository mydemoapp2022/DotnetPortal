using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace UI.EmployerPortal.Web.Features.Shared.Layout;

/// <summary>
/// SideMenu
/// </summary>
public partial class SideMenu : IDisposable
{
    private bool _isMinimized = false;
    private readonly HashSet<string> _expandedMenus = [];

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    /// <summary>
    /// OnInitialized
    /// </summary>
    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }

    private static readonly List<MenuItem> MenuItems =
    [
        new MenuItem { Id = "dashboard", Title = "Dashboard", Icon = "images/dashboard/dashboard.svg", Url = "/employer-dashboard", HasSubmenu = false },
        new MenuItem { Id = "employer-registration", Title = "Employer Registration", Icon = "images/dashboard/manage-account.svg", Url = "/employer-registration", HasSubmenu = false },
        new MenuItem { Id = "eforms", Title = "eForms", Icon = "images/dashboard/eforms.svg", Url = "/eforms", HasSubmenu = false },
        new MenuItem { Id = "secure-messages", Title = "Secure Messages", Icon = "images/dashboard/secure-messages.svg", Url = "/secure-messages", HasSubmenu = false },
        new MenuItem { Id = "quarterly-tax", Title = "Quarterly Tax & Wage Reporting", Icon = "images/dashboard/quarterly-tax.svg", Url = "/quarterly-tax", HasSubmenu = true, Submenus = new List<SubMenuItem>
        {
            new() { Title = "Missing Reports", Url = "/quarterly-tax/missing-reports" },
            new() { Title = "Quarterly Tax & Wage Reports", Url = "/quarterly-tax/reports" },
            new() { Title = "Wage Report File Upload", Url = "/quarterly-tax/file-upload" },
            new() { Title = "Tax & Wage Report Adjustments", Url = "/quarterly-tax/adjustments" },
            new() { Title = "Previously Filed Reports", Url = "/quarterly-tax/previously-filed" },
            new() { Title = "First Quarter Deferral", Url = "/quarterly-tax/first-quarter-deferral" },
            new() { Title = "Validation Test Environment", Url = "/quarterly-tax/validation-test" },
            new() { Title = "View File Upload Summary", Url = "/quarterly-tax/upload-summary" },
            new() { Title = "View Rate Summary", Url = "/quarterly-tax/rate-summary" }
        }},
        new MenuItem { Id = "billing-payments", Title = "Billing & Payments", Icon = "images/dashboard/billing-payments.svg", Url = "/billing-payments", HasSubmenu = true, Submenus = new List<SubMenuItem>
        {
            new() { Title = "View Account Balance", Url = "/billing-payments/balance" },
            new() { Title = "Make a Payment", Url = "/billing-payments/make-payment" },
            new() { Title = "Payment History", Url = "/billing-payments/history" }
        }},
        new MenuItem { Id = "ui-documents", Title = "UI Documents", Icon = "images/dashboard/ui-documents.svg", Url = "/ui-documents", HasSubmenu = false },
        new MenuItem { Id = "levies", Title = "Levies", Icon = "images/dashboard/levies.svg", Url = "/levies", HasSubmenu = true, Submenus = new List<SubMenuItem>
        {
            new() { Title = "Active Levies", Url = "/levies/active" },
            new() { Title = "Levy Payments", Url = "/levies/payments" }
        }},
        new MenuItem { Id = "appeals", Title = "Appeals", Icon = "images/dashboard/appeals.svg", Url = "/appeals", HasSubmenu = true, Submenus = new List<SubMenuItem>
        {
            new() { Title = "File an Appeal", Url = "/appeals/file" },
            new() { Title = "Upload Hearing Documentation", Url = "/appeals/upload-docs" },
            new() { Title = "Update Hearing Contact Info", Url = "/appeals/contact-info" }
        }},
        new MenuItem { Id = "manage-account", Title = "View & Manage Account", Icon = "images/dashboard/manage-account.svg", Url = "/manage-account", HasSubmenu = true, Submenus = new List<SubMenuItem>
        {
            new() { Title = "View Account Information", Url = "/manage-account/view" },
            new() { Title = "Request Account Access", Url = "/manage-account/request-access" },
            new() { Title = "Manage Worker Access", Url = "/manage-account/worker-access" },
            new() { Title = "Employer Registration", Url = "/manage-account/registration" },
            new() { Title = "Manage Service Providers", Url = "/manage-account/service-providers" }
        }},
        new MenuItem { Id = "other-functions", Title = "Other Functions", Icon = "images/dashboard/other-functions.svg", Url = "/other-functions", HasSubmenu = true, Submenus = new List<SubMenuItem>
        {
            new() { Title = "Download Benefit Charges", Url = "/other-functions/benefit-charges" },
            new() { Title = "Request 940C Recertification", Url = "/other-functions/940c" },
            new() { Title = "Manage Email Subscriptions", Url = "/other-functions/email-subscriptions" },
            new() { Title = "Upload Audit Files", Url = "/other-functions/upload-audit" }
        }}
    ];

    private readonly List<MenuItem> _menuItems = MenuItems;

    private void ToggleMenu()
    {
        _isMinimized = !_isMinimized;
        if (_isMinimized)
        {
            _expandedMenus.Clear();
        }
    }

    private void OnMenuClick(MenuItem item)
    {
        if (item.HasSubmenu)
        {
            if (_expandedMenus.Contains(item.Id))
            {
                _expandedMenus.Remove(item.Id);
            }
            else
            {
                _expandedMenus.Add(item.Id);
            }
        }
        else
        {
            NavigationManager.NavigateTo(item.Url);
        }
    }

    private bool IsMenuActive(MenuItem item)
    {
        var currentUri = NavigationManager.Uri;
        var relativePath = NavigationManager.ToBaseRelativePath(currentUri).Split('?')[0].TrimEnd('/').ToLower();
        var itemPath = item.Url.TrimStart('/').ToLower();

        if (item.HasSubmenu)
        {
            // Only highlight if one of the submenus matches the current URL
            return item.Submenus.Any(s =>
            {
                return relativePath.StartsWith(s.Url.TrimStart('/').ToLower(), StringComparison.OrdinalIgnoreCase);
            });
        }

        // For items without submenus, highlight if URL matches or starts with the item URL
        return relativePath == itemPath || relativePath.StartsWith(itemPath + "/");
    }

    private bool IsSubmenuActive(SubMenuItem submenu)
    {
        var currentUri = NavigationManager.Uri;
        var relativePath = NavigationManager.ToBaseRelativePath(currentUri);
        return relativePath.Equals(submenu.Url.TrimStart('/'), StringComparison.OrdinalIgnoreCase);
    }

    private class MenuItem
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Url { get; set; } = "";
        public bool HasSubmenu { get; set; }
        public List<SubMenuItem> Submenus { get; set; } = new List<SubMenuItem>();
    }

    private class SubMenuItem
    {
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
    }
}
