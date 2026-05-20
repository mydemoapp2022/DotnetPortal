using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using UI.EmployerPortal.Web.Features.BillingPayments.Models;

namespace UI.EmployerPortal.Web.Features.BillingPayments.Pages;

/// <summary>
/// Code-behind for the Manage Bank Accounts page.
/// Controls navigation between the account list, add form, and save confirmation states.
/// </summary>
public partial class ManageBankAccounts
{
    [Inject] private IBankAccountOrchestrator BankAccountOrchestrator { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private BankAccountPageState _pageState = BankAccountPageState.List;
    private SavedBankAccount? _savedAccount;
    private IReadOnlyList<SavedBankAccount> _accounts = [];
    private bool _isLoading;
    private string? _loadError;
    private string _sortColumn = "nickname";
    private bool _sortAscending = true;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await LoadAccountsAsync();
    }

    private async Task LoadAccountsAsync()
    {
        _isLoading = true;
        _loadError = null;

        try
        {
            _accounts = await BankAccountOrchestrator.GetExistingAccountsAsync();
        }
        catch (Exception)
        {
            _loadError = "Unable to load bank accounts. Please try again.";
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void HandleAddAccount()
    {
        _pageState = BankAccountPageState.AddForm;
    }

    private void HandleSaved(SavedBankAccount account)
    {
        _savedAccount = account;
        _pageState = BankAccountPageState.Confirmation;
    }

    private async Task HandleBackToList()
    {
        _savedAccount = null;
        _pageState = BankAccountPageState.List;
        await LoadAccountsAsync();
    }

    private void HandleContinue()
    {
        NavigationManager.NavigateTo("/billing-payments");
    }

    private IEnumerable<SavedBankAccount> GetSortedAccounts()
    {
        return _sortColumn switch
        {
            "nickname" => SortBy(a =>
            {
                return a.Nickname;
            }),
            "bankName" => SortBy(a =>
            {
                return a.BankName;
            }),
            "accountType" => SortBy(a =>
            {
                return a.AccountType;
            }),
            "accountNumber" => SortBy(a =>
            {
                return a.MaskedAccountNumber;
            }),
            "routingNumber" => SortBy(a =>
            {
                return a.RoutingNumber;
            }),
            _ => SortBy(a =>
            {
                return a.Nickname;
            }),
        };
    }

    private IOrderedEnumerable<SavedBankAccount> SortBy<TKey>(Func<SavedBankAccount, TKey> keySelector)
    {
        return _sortAscending
            ? _accounts.OrderBy(keySelector)
            : _accounts.OrderByDescending(keySelector);
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
    }

    private MarkupString GetSortIcon(string column)
    {
        string path;
        string altText;

        if (_sortColumn == column)
        {
            path = _sortAscending ? "images/sort/sort-icon-asc.svg" : "images/sort/sort-icon-desc.svg";
            altText = _sortAscending ? "Sorted ascending" : "Sorted descending";
        }
        else
        {
            path = "images/sort/sort-icon.svg";
            altText = "Not sorted";
        }

        return new MarkupString($"<img src='{Assets[path]}' class='sort-icon' alt='{altText}' />");
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
}

/// <summary>
/// Represents the current display state of the Manage Bank Accounts page.
/// </summary>
internal enum BankAccountPageState
{
    /// <summary>
    /// The account list view with the Add Account button.
    /// </summary>
    List,

    /// <summary>
    /// The blank Bank Information form for adding a new account.
    /// </summary>
    AddForm,

    /// <summary>
    /// The save confirmation screen shown after a successful add.
    /// </summary>
    Confirmation
}
