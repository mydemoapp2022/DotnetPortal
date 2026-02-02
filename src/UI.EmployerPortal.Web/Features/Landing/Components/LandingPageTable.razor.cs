using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Models;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Services;

namespace UI.EmployerPortal.Web.Features.Landing.Components;

/// <summary>
/// LandingPageTable
/// </summary>
public partial class LandingPageTable
{
    [Inject]
    private IAccountService AccountService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private ILandingOrchestrator LandingOrchestrator { get; set; } = default!;

    private List<Account> _accounts = new();
    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalCount;
    private int TotalPages => (int) Math.Ceiling((double) _totalCount / _pageSize);
    private string _sortColumn = "legalName";
    private bool _sortAscending = true;


    /// <summary>
    /// OnInitialized
    /// </summary>
    protected override void OnInitialized()
    {
        LoadData();
    }

    private void LoadData()
    {
        var (data, totalCount) = AccountService.GetAccounts(_currentPage, _pageSize, _sortColumn, _sortAscending);
        _accounts = data;
        _totalCount = totalCount;
    }

    private void Sort(string column)
    {
        if (_sortColumn == column)
        {
            _sortAscending = !_sortAscending;
        }
        else
        {
            _sortColumn = column;
            _sortAscending = true;
        }
        _currentPage = 1;
        LoadData();
    }

    private MarkupString GetSortIcon(string column)
    {
        string icon;
        string altText;

        if (_sortColumn == column)
        {
            icon = _sortAscending ? "/images/sort/sort-icon-asc.svg" : "/images/sort/sort-icon-desc.svg";
            altText = _sortAscending ? "Sorted ascending" : "Sorted descending";
        }
        else
        {
            icon = "/images/sort/sort-icon.svg";
            altText = "Not sorted";
        }

        return new MarkupString($"<img src='{icon}' class='sort-icon' alt='{altText}' />");
    }

    private string? GetAriaSort(string column)
    {
        return _sortColumn != column ? null : _sortAscending ? "ascending" : "descending";
    }

    private void HandleHeaderKeyDown(KeyboardEventArgs e, string column)
    {
        if (e.Key is "Enter" or " ")
        {
            Sort(column);
        }
    }

    private async Task HandleRowKeyDown(KeyboardEventArgs e, Account account)
    {
        if (e.Key is "Enter" or " ")
        {
            await NavigateToEmployerDashboard(account);
        }
    }

    private void OnPageSizeChanged(ChangeEventArgs e)
    {
        _pageSize = int.Parse(e.Value?.ToString() ?? "10");
        _currentPage = 1;
        LoadData();
    }

    private void FirstPage()
    {
        if (_currentPage > 1)
        {
            _currentPage = 1;
            LoadData();
        }
    }

    private void PreviousPage()
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            LoadData();
        }
    }

    private void NextPage()
    {
        if (_currentPage < TotalPages)
        {
            _currentPage++;
            LoadData();
        }
    }

    private void LastPage()
    {
        if (_currentPage < TotalPages)
        {
            _currentPage = TotalPages;
            LoadData();
        }
    }

    private async Task NavigateToEmployerDashboard(Account account)
    {
        await LandingOrchestrator.SelectAccountAsync(account);
        NavigationManager.NavigateTo("/employer-dashboard");
    }
}
