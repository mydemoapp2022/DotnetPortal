using UI.EmployerPortal.Web.Features.Shared.Accounts.Models;

namespace UI.EmployerPortal.Web.Features.Shared.Accounts.Services;

/// <summary>
/// IAccountService
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// GetAllAccounts
    /// </summary>
    /// <returns></returns>
    List<Account> GetAllAccounts();

    /// <summary>
    /// GetAccounts()
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortAscending"></param>
    /// <returns></returns>
    (List<Account> Data, int TotalCount) GetAccounts(int page, int pageSize, string? sortColumn, bool sortAscending);
}

/// <summary>
/// Temp Account Service for Demo
/// </summary>
public class AccountService : IAccountService
{
    private readonly List<Account> _accounts;

    /// <summary>
    /// Constructor
    /// </summary>
    public AccountService()
    {
        _accounts = GenerateSampleData();
    }

    /// <summary>
    /// GetAllAccounts
    /// </summary>
    /// <returns></returns>
    public List<Account> GetAllAccounts()
    {
        return _accounts;
    }

    /// <summary>
    /// GetAccounts
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortAscending"></param>
    /// <returns></returns>
    public (List<Account> Data, int TotalCount) GetAccounts(int page, int pageSize, string? sortColumn, bool sortAscending)
    {
        var query = _accounts.AsQueryable();

        // Apply sorting
        query = sortColumn?.ToLower() switch
        {
            "legalname" => sortAscending ? query.OrderBy(a => a.LegalName) : query.OrderByDescending(a => a.LegalName),
            "accountno" => sortAscending ? query.OrderBy(a => a.AccountNo) : query.OrderByDescending(a => a.AccountNo),
            "balancedue" => sortAscending ? query.OrderBy(a => a.BalanceDue) : query.OrderByDescending(a => a.BalanceDue),
            "missingquarterlyreports" => sortAscending ? query.OrderBy(a => a.MissingQuarterlyReports) : query.OrderByDescending(a => a.MissingQuarterlyReports),
            "pendingtaxeforms" => sortAscending ? query.OrderBy(a => a.PendingTaxEForms) : query.OrderByDescending(a => a.PendingTaxEForms),
            "pendingbenefitseforms" => sortAscending ? query.OrderBy(a => a.PendingBenefitsEForms) : query.OrderByDescending(a => a.PendingBenefitsEForms),
            "unreadmessages" => sortAscending ? query.OrderBy(a => a.UnreadMessages) : query.OrderByDescending(a => a.UnreadMessages),
            _ => query.OrderBy(a => a.LegalName)
        };

        var totalCount = _accounts.Count;
        var data = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return (data, totalCount);
    }

    private static List<Account> GenerateSampleData()
    {
        return
        [
            new() { Id = 1, LegalName = "Alpha Solutions Corp", AccountNo = "222333-444-5", BalanceDue = 8920.00m, MissingQuarterlyReports = 0, PendingTaxEForms = 2, PendingBenefitsEForms = 1, UnreadMessages = 0 },
            new() { Id = 2, LegalName = "Beta Industries LLC", AccountNo = "333444-555-6", BalanceDue = -2100.00m, MissingQuarterlyReports = 1, PendingTaxEForms = 0, PendingBenefitsEForms = 0, UnreadMessages = 5 },
            new() { Id = 3, LegalName = "Gamma Enterprises", AccountNo = "444555-666-7", BalanceDue = 45000.00m, MissingQuarterlyReports = 0, PendingTaxEForms = 0, PendingBenefitsEForms = 3, UnreadMessages = 1 },
            new() { Id = 4, LegalName = "Delta Services Inc.", AccountNo = "555666-777-8", BalanceDue = 0.00m, MissingQuarterlyReports = 3, PendingTaxEForms = 1, PendingBenefitsEForms = 2, UnreadMessages = 0 },
            new() { Id = 5, LegalName = "Epsilon Holdings", AccountNo = "666777-888-9", BalanceDue = 22340.75m, MissingQuarterlyReports = 0, PendingTaxEForms = 0, PendingBenefitsEForms = 0, UnreadMessages = 2 },
            new() { Id = 6, LegalName = "Zeta Technologies", AccountNo = "777888-999-0", BalanceDue = 15000.00m, MissingQuarterlyReports = 2, PendingTaxEForms = 1, PendingBenefitsEForms = 0, UnreadMessages = 3 },
            new() { Id = 7, LegalName = "Eta Manufacturing", AccountNo = "888999-000-1", BalanceDue = -500.00m, MissingQuarterlyReports = 0, PendingTaxEForms = 3, PendingBenefitsEForms = 1, UnreadMessages = 0 },
            new() { Id = 8, LegalName = "Theta Consulting", AccountNo = "999000-111-2", BalanceDue = 33250.50m, MissingQuarterlyReports = 1, PendingTaxEForms = 0, PendingBenefitsEForms = 2, UnreadMessages = 4 },
            new() { Id = 9, LegalName = "Iota Financial", AccountNo = "000111-222-3", BalanceDue = 0.00m, MissingQuarterlyReports = 0, PendingTaxEForms = 2, PendingBenefitsEForms = 0, UnreadMessages = 1 },
            new() { Id = 10, LegalName = "Kappa Retail Group", AccountNo = "111222-333-4", BalanceDue = 67890.25m, MissingQuarterlyReports = 4, PendingTaxEForms = 0, PendingBenefitsEForms = 3, UnreadMessages = 0 },
            new() { Id = 11, LegalName = "Lambda Healthcare", AccountNo = "123456-789-0", BalanceDue = 12500.00m, MissingQuarterlyReports = 0, PendingTaxEForms = 1, PendingBenefitsEForms = 4, UnreadMessages = 2 },
            new() { Id = 12, LegalName = "Mu Construction Co", AccountNo = "234567-890-1", BalanceDue = -3200.00m, MissingQuarterlyReports = 2, PendingTaxEForms = 0, PendingBenefitsEForms = 0, UnreadMessages = 6 },
            new() { Id = 13, LegalName = "Nu Energy Systems", AccountNo = "345678-901-2", BalanceDue = 89000.00m, MissingQuarterlyReports = 0, PendingTaxEForms = 2, PendingBenefitsEForms = 1, UnreadMessages = 0 },
            new() { Id = 14, LegalName = "Xi Logistics Inc", AccountNo = "456789-012-3", BalanceDue = 4500.75m, MissingQuarterlyReports = 1, PendingTaxEForms = 3, PendingBenefitsEForms = 0, UnreadMessages = 3 },
            new() { Id = 15, LegalName = "Omicron Software", AccountNo = "567890-123-4", BalanceDue = 0.00m, MissingQuarterlyReports = 0, PendingTaxEForms = 0, PendingBenefitsEForms = 2, UnreadMessages = 1 },
            new() { Id = 16, LegalName = "Pi Research Labs", AccountNo = "678901-234-5", BalanceDue = 27800.00m, MissingQuarterlyReports = 3, PendingTaxEForms = 1, PendingBenefitsEForms = 0, UnreadMessages = 0 },
            new() { Id = 17, LegalName = "Rho Media Group", AccountNo = "789012-345-6", BalanceDue = -1800.50m, MissingQuarterlyReports = 0, PendingTaxEForms = 0, PendingBenefitsEForms = 3, UnreadMessages = 4 },
            new() { Id = 18, LegalName = "Sigma Automotive", AccountNo = "890123-456-7", BalanceDue = 56000.00m, MissingQuarterlyReports = 1, PendingTaxEForms = 2, PendingBenefitsEForms = 1, UnreadMessages = 2 }
        ];
    }
}
