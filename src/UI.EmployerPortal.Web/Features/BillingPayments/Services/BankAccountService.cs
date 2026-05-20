using System.ServiceModel;
using UI.EmployerPortal.Generated.ServiceClients.EFTPaymentService;
using UI.EmployerPortal.Web.Features.BillingPayments.Models;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Services;
using UI.EmployerPortal.Web.Startup.ResiliencyProtocols;

namespace UI.EmployerPortal.Web.Features.BillingPayments.Services;

internal class BankAccountService : IBankAccountService
{
    private const int AccountTypeChecking = 1;
    private const int AccountTypeSavings = 2;

    private readonly IEFTPaymentService _eftPaymentService;
    private readonly IUserAccountService _userAccountService;
    private readonly IAsyncRetryPolicy<BankAccountService> _retryPolicy;

    public BankAccountService(
        IEFTPaymentService eftPaymentService,
        IUserAccountService userAccountService,
        IAsyncRetryPolicy<BankAccountService> retryPolicy)
    {
        _eftPaymentService = eftPaymentService;
        _userAccountService = userAccountService;
        _retryPolicy = retryPolicy;
    }

    public async Task<string?> CheckRoutingNumberAsync(string routingNumber)
    {
        try
        {
            var response = await _retryPolicy.ExecuteAsync(() =>
            {
                return _eftPaymentService.CheckBankRoutingNumberAsync(routingNumber);
            });

            return response?.BankName;
        }
        catch (CommunicationException)
        {
            return null;
        }
    }

    public async Task<SaveBankAccountResult> SaveBankAccountAsync(BankAccountModel model, int employerAccountSk)
    {
        try
        {
            var request = new BankAccountRequest
            {
                SecureUserSK = _userAccountService.GetUserSKClaim(),
                EmployerSK = employerAccountSk,
                NickName = model.Nickname,
                RoutingNumber = model.RoutingNumber,
                AccountNumber = model.AccountNumber,
                AccountType = MapAccountType(model.AccountType),
                OffshoreFlag = model.IsInternational,
                IATStreetAddress = model.IatStreetAddress,
                IATCity = model.IatCity,
                IATZip = model.IatPostalCode,
                IATCountryCode = model.IatCountryCode,
                IATState = 0
            };

            var response = await _retryPolicy.ExecuteAsync(() =>
            {
                return _eftPaymentService.SaveBankAccountAsync(request);
            });

            if (response?.RuleViolations != null && response.RuleViolations.Length > 0)
            {
                var firstViolation = response.RuleViolations[0].RuleViolation ?? "Save failed. Please try again.";
                return new SaveBankAccountResult(false, firstViolation);
            }

            return new SaveBankAccountResult(true);
        }
        catch (CommunicationException)
        {
            return new SaveBankAccountResult(false, "Service is temporarily unavailable. Please try again.");
        }
        catch (Exception)
        {
            return new SaveBankAccountResult(false, "An unexpected error occurred. Please try again.");
        }
    }

    public async Task<IReadOnlyList<SavedBankAccount>> GetExistingAccountsAsync(int employerAccountSk)
    {
        try
        {
            var secureUserSk = _userAccountService.GetUserSKClaim();

            var response = await _retryPolicy.ExecuteAsync(() =>
            {
                return _eftPaymentService.LoadActiveBankAccountAsync(employerAccountSk, secureUserSk);
            });

            if (response?.BankAccounts == null || response.BankAccounts.Length == 0)
            {
                return [];
            }

            var accounts = new List<SavedBankAccount>();

            foreach (var account in response.BankAccounts)
            {
                accounts.Add(new SavedBankAccount
                {
                    Nickname = account.Nickname ?? string.Empty,
                    RoutingNumber = account.RoutingNumber ?? string.Empty,
                    MaskedAccountNumber = account.AccountNumberMasked ?? string.Empty,
                    BankName = account.FederalBankName ?? string.Empty,
                    AccountType = MapAccountTypeToString(account.AccountType)
                });
            }

            return accounts;
        }
        catch (CommunicationException)
        {
            return [];
        }
    }

    private static int MapAccountType(string? accountType)
    {
        return string.Equals(accountType, "Savings", StringComparison.OrdinalIgnoreCase) ? AccountTypeSavings : AccountTypeChecking;
    }

    private static string MapAccountTypeToString(int accountType)
    {
        return accountType == AccountTypeSavings ? "Savings" : "Checking";
    }
}
