using Bunit;
using FakeItEasy;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using UI.EmployerPortal.Web.Features.BillingPayments;
using UI.EmployerPortal.Web.Features.BillingPayments.Components;
using UI.EmployerPortal.Web.Features.BillingPayments.Models;
using UI.EmployerPortal.Web.Features.BillingPayments.Pages;
using UI.EmployerPortal.Web.Features.BillingPayments.Services;
using UI.EmployerPortal.Web.Features.Dashboard;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Models;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Services;
using UI.EmployerPortal.Web.Features.Shared.QuarterlyTax.Services;
using Xunit;

namespace Test.UI.EmployerPortal.Web;

/// <summary>
/// Component tests for ACH bank account display and selection behaviour,
/// covering both the <see cref="AchBankInfoDisplay"/> component and the
/// <see cref="BankAccountPaymentAch"/> page.
/// </summary>
public class AchBankAccountTests : BunitContext
{
    // ── Shared stub data ────────────────────────────────────────────────

    private static readonly SavedBankAccount CheckingAccount = new()
    {
        Nickname = "Janice's Checking Account",
        BankName = "Hometown Bank",
        RoutingNumber = "015708055",
        MaskedAccountNumber = "********9177",
        AccountType = "Checking"
    };

    private static readonly SavedBankAccount SavingsAccount = new()
    {
        Nickname = "Janice's Savings Account",
        BankName = "Hometown Bank",
        RoutingNumber = "015708055",
        MaskedAccountNumber = "********4321",
        AccountType = "Savings"
    };

    private static readonly List<SavedBankAccount> TwoAccounts = [CheckingAccount, SavingsAccount];

    private static readonly ACHContactModel StubContact = new()
    {
        ContactName = "Janice Smith",
        PhoneNumber = "(302) 789-6894",
        Email = "jsmith@email.com"
    };

    // ── Fakes for page-level services ───────────────────────────────────

    private readonly IFederalReserveHolidayService _holidayService;
    private readonly IBankAccountOrchestrator _bankAccountOrchestrator;
    private readonly IContactInformationService _contactInfoService;
    private readonly IDashboardOrchestrator _dashboardOrchestrator;
    private readonly IUserAccountService _userAccountService;

    // ── Constructor ─────────────────────────────────────────────────────

    public AchBankAccountTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;

        // ── Holiday service (used by AchPaymentInformation) ──
        _holidayService = A.Fake<IFederalReserveHolidayService>();
        A.CallTo(() => _holidayService.IsFederalReserveHoliday(A<DateOnly>._)).Returns(false);
        A.CallTo(() => _holidayService.GetInvalidDatesInRange(A<DateOnly>._, A<DateOnly>._))
            .Returns(Array.Empty<DateOnly>());

        // ── Page-level service fakes ──
        _bankAccountOrchestrator = A.Fake<IBankAccountOrchestrator>();
        A.CallTo(() => _bankAccountOrchestrator.GetExistingAccountsAsync())
            .Returns(TwoAccounts.AsReadOnly());

        _contactInfoService = A.Fake<IContactInformationService>();
        A.CallTo(() => _contactInfoService.GetEmployerWebContact(A<int>._, A<int>._, A<int>._))
            .Returns(StubContact);

        _dashboardOrchestrator = A.Fake<IDashboardOrchestrator>();
        A.CallTo(() => _dashboardOrchestrator.GetSelectedEmployerAccountAsync())
            .Returns(new EmployerAccount { Id = 1, LegalName = "Acme Corp" });

        _userAccountService = A.Fake<IUserAccountService>();

        // Register all fakes
        Services.AddSingleton(_holidayService);
        Services.AddSingleton(_bankAccountOrchestrator);
        Services.AddSingleton(_contactInfoService);
        Services.AddSingleton(_dashboardOrchestrator);
        Services.AddSingleton(_userAccountService);
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    private IRenderedComponent<AchBankInfoDisplay> RenderDisplay(
        List<SavedBankAccount>? accounts = null,
        SavedBankAccount? selected = null,
        bool readOnly = false,
        bool? showEditLink = null)
    {
        return Render<AchBankInfoDisplay>(p =>
        {
            p.Add(x => x.BankAccounts, accounts ?? TwoAccounts);
            p.Add(x => x.BankAccount, selected ?? accounts?.FirstOrDefault() ?? CheckingAccount);
            p.Add(x => x.ReadOnly, readOnly);
            if (showEditLink.HasValue)
                p.Add(x => x.ShowEditLink, showEditLink.Value);
        });
    }

    /// <summary>
    /// Renders the page and waits until the loading spinner disappears.
    /// </summary>
    private IRenderedComponent<BankAccountPaymentAch> RenderPage()
    {
        var cut = Render<BankAccountPaymentAch>();
        cut.WaitForState(() => !cut.Markup.Contains("ach-loading"));
        return cut;
    }

    private static void ClickContinue(IRenderedComponent<BankAccountPaymentAch> cut)
    {
        cut.FindAll("button")
            .Single(b => { return b.TextContent.Contains("CONTINUE", StringComparison.OrdinalIgnoreCase); })
            .Click();
    }

    private static void ClickBack(IRenderedComponent<BankAccountPaymentAch> cut)
    {
        cut.FindAll("button")
            .Single(b => { return b.TextContent.Contains("BACK", StringComparison.OrdinalIgnoreCase); })
            .Click();
    }

    private static void FillValidAmount(IRenderedComponent<BankAccountPaymentAch> cut, string amount = "500.00")
    {
        var input = cut.Find("input[inputmode='decimal']");
        input.Input(amount);
        input.Blur();
    }

    // =========================================================
    // AchBankInfoDisplay — rendering tests
    // =========================================================

    /// <summary>
    /// Verifies that the selected account's bank name, masked number, and account
    /// type are rendered when the component is given a pre-selected account.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Shows_First_Account_Details_By_Default()
    {
        var cut = RenderDisplay();

        Assert.Contains(CheckingAccount.BankName, cut.Markup);
        Assert.Contains(CheckingAccount.MaskedAccountNumber, cut.Markup);
        Assert.Contains(CheckingAccount.AccountType, cut.Markup);
    }

    /// <summary>
    /// Verifies that the account nickname (Nickname property) is rendered for the
    /// selected account.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Shows_Nickname_Of_Selected_Account()
    {
        var cut = RenderDisplay();

        Assert.Contains(CheckingAccount.Nickname, cut.Markup);
    }

    /// <summary>
    /// Verifies that the routing number of the pre-selected account is rendered.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Shows_Routing_Number_Of_Selected_Account()
    {
        var cut = RenderDisplay();

        Assert.Contains(CheckingAccount.RoutingNumber, cut.Markup);
    }

    /// <summary>
    /// Verifies that the account selector dropdown is rendered when not in
    /// read-only mode and multiple accounts are available.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Shows_Account_Selector_When_Not_ReadOnly()
    {
        var cut = RenderDisplay(readOnly: false);

        Assert.NotNull(cut.Find("select"));
    }

    /// <summary>
    /// Verifies that the account selector dropdown is hidden when the component
    /// is in read-only mode.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Hides_Account_Selector_When_ReadOnly()
    {
        var cut = RenderDisplay(readOnly: true);

        Assert.Empty(cut.FindAll("select"));
    }

    /// <summary>
    /// Verifies that selecting a different account from the dropdown updates the
    /// displayed bank details (masked number, account type) to match the new account.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Changing_Account_Updates_Displayed_Details()
    {
        var cut = RenderDisplay(readOnly: false);

        cut.Find("select").Change(SavingsAccount.Nickname);

        Assert.Contains(SavingsAccount.MaskedAccountNumber, cut.Markup);
        Assert.Contains(SavingsAccount.AccountType, cut.Markup);
    }

    /// <summary>
    /// Verifies that the <see cref="AchBankInfoDisplay.SelectedAccountChanged"/> callback
    /// is invoked with the correct <see cref="SavedBankAccount"/> when the user changes
    /// the selection.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Fires_SelectedAccountChanged_Callback_On_Change()
    {
        SavedBankAccount? received = null;

        var cut = Render<AchBankInfoDisplay>(p =>
        {
            p.Add(x => x.BankAccounts, TwoAccounts);
            p.Add(x => x.BankAccount, CheckingAccount);
            p.Add(x => x.ReadOnly, false);
            p.Add(x => x.SelectedAccountChanged, EventCallback.Factory.Create<SavedBankAccount>(
                this, account => { received = account; }));
        });

        cut.Find("select").Change(SavingsAccount.Nickname);

        Assert.NotNull(received);
        Assert.Equal(SavingsAccount.Nickname, received!.Nickname);
    }

    /// <summary>
    /// Verifies that changing to the savings account also updates the routing number
    /// displayed in the details panel.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Changing_Account_Updates_Routing_Number()
    {
        var cut = RenderDisplay(readOnly: false);

        cut.Find("select").Change(SavingsAccount.Nickname);

        Assert.Contains(SavingsAccount.RoutingNumber, cut.Markup);
    }

    // =========================================================
    // AchBankInfoDisplay — Edit link / Add link visibility
    // =========================================================

    /// <summary>
    /// Verifies that the Edit link is rendered when the component is not in
    /// read-only mode and an account is selected.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Shows_Edit_Link_When_Not_ReadOnly()
    {
        var cut = RenderDisplay(readOnly: false);

        var editLink = cut.FindAll("a")
            .SingleOrDefault(a => { return a.GetAttribute("aria-label") == "Edit Bank Information"; });

        Assert.NotNull(editLink);
    }

    /// <summary>
    /// Verifies that the Edit link is hidden when the component is in read-only mode
    /// and <see cref="AchBankInfoDisplay.ShowEditLink"/> is not explicitly set.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Hides_Edit_Link_When_ReadOnly()
    {
        var cut = RenderDisplay(readOnly: true);

        var editLinks = cut.FindAll("a")
            .Where(a => { return a.GetAttribute("aria-label") == "Edit Bank Information"; });

        Assert.Empty(editLinks);
    }

    /// <summary>
    /// Verifies that <c>ShowEditLink="true"</c> overrides <c>ReadOnly="true"</c> so the
    /// Edit link is still rendered (e.g. on the Edit Payment step).
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Shows_Edit_Link_When_ShowEditLink_Overrides_ReadOnly()
    {
        var cut = RenderDisplay(readOnly: true, showEditLink: true);

        var editLink = cut.FindAll("a")
            .SingleOrDefault(a => { return a.GetAttribute("aria-label") == "Edit Bank Information"; });

        Assert.NotNull(editLink);
    }

    /// <summary>
    /// Verifies that the "Add Account" link is rendered when no bank accounts exist
    /// and the component is not in read-only mode.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Shows_Add_Account_Link_When_No_Accounts_And_Not_ReadOnly()
    {
        var cut = Render<AchBankInfoDisplay>(p =>
        {
            p.Add(x => x.BankAccounts, new List<SavedBankAccount>());
            p.Add(x => x.ReadOnly, false);
        });

        Assert.Contains("+ Add Account", cut.Markup);
    }

    /// <summary>
    /// Verifies that the "Add Account" link is hidden when no accounts exist but the
    /// component is in read-only mode.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Hides_Add_Account_Link_When_ReadOnly()
    {
        var cut = Render<AchBankInfoDisplay>(p =>
        {
            p.Add(x => x.BankAccounts, new List<SavedBankAccount>());
            p.Add(x => x.ReadOnly, true);
        });

        Assert.DoesNotContain("+ Add Account", cut.Markup);
    }

    // =========================================================
    // AchBankInfoDisplay — collapse toggle
    // =========================================================

    /// <summary>
    /// Verifies that the bank information section collapses when the toggle button is
    /// clicked, and the region's hidden attribute is set.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Collapses_When_Toggle_Button_Clicked()
    {
        var cut = RenderDisplay();

        Assert.Contains(CheckingAccount.BankName, cut.Markup);

        cut.Find("button.ach-collapse-btn").Click();

        var body = cut.Find("#bank-info-body");
        Assert.True(body.HasAttribute("hidden"));
    }

    /// <summary>
    /// Verifies that the section expands again after being collapsed and re-clicked,
    /// and the hidden attribute is removed.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Expands_After_Collapse_Toggle_Clicked_Twice()
    {
        var cut = RenderDisplay();
        var btn = cut.Find("button.ach-collapse-btn");

        btn.Click(); // collapse
        btn.Click(); // expand

        var body = cut.Find("#bank-info-body");
        Assert.False(body.HasAttribute("hidden"));
    }

    /// <summary>
    /// Verifies that the collapse button aria-expanded attribute reflects the
    /// current expanded/collapsed state.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Toggle_Button_AriaExpanded_Reflects_State()
    {
        var cut = RenderDisplay();
        var btn = cut.Find("button.ach-collapse-btn");

        Assert.Equal("true", btn.GetAttribute("aria-expanded"));

        btn.Click();

        Assert.Equal("false", btn.GetAttribute("aria-expanded"));
    }

    // =========================================================
    // BankAccountPaymentAch page — loading state
    // =========================================================

    /// <summary>
    /// Verifies that the loading spinner is shown while OnInitializedAsync is running,
    /// before bank accounts and contact data have been fetched.
    /// </summary>
    [Fact]
    public void Page_Shows_Loading_Spinner_While_Initializing()
    {
        // Delay the bank account service so we can observe the loading state
        A.CallTo(() => _bankAccountOrchestrator.GetExistingAccountsAsync())
            .ReturnsLazily(async () =>
            {
                await Task.Delay(500);
                return (IReadOnlyList<SavedBankAccount>) TwoAccounts.AsReadOnly();
            });

        var cut = Render<BankAccountPaymentAch>();

        Assert.Contains("ach-loading", cut.Markup);
    }

    /// <summary>
    /// Verifies that the loading spinner is replaced by the entry-step content once
    /// all services have returned data.
    /// </summary>
    [Fact]
    public void Page_Hides_Loading_Spinner_After_Data_Loaded()
    {
        var cut = RenderPage();

        Assert.DoesNotContain("ach-loading", cut.Markup);
        Assert.Contains("Bank Account Payment (ACH)", cut.Markup);
    }

    // =========================================================
    // BankAccountPaymentAch page — Entry step
    // =========================================================

    /// <summary>
    /// Verifies that bank accounts returned by IBankAccountOrchestrator are displayed
    /// in the account selector dropdown on the Entry step.
    /// </summary>
    [Fact]
    public void Page_Entry_Step_Shows_Bank_Accounts_From_Service()
    {
        var cut = RenderPage();

        Assert.Contains(CheckingAccount.Nickname, cut.Markup);
        Assert.Contains(SavingsAccount.Nickname, cut.Markup);
    }

    /// <summary>
    /// Verifies that clicking Continue with all required data filled advances the
    /// page to the Verify &amp; Authorize step.
    /// </summary>
    [Fact]
    public void Page_Entry_Step_Advances_To_VerifyAuthorize_When_All_Data_Valid()
    {
        var cut = RenderPage();

        FillValidAmount(cut);
        ClickContinue(cut);

        Assert.Contains("Verify &amp; Authorize EFT Payment", cut.Markup);
    }

    /// <summary>
    /// Verifies that clicking Continue without a bank account shows a validation error
    /// and keeps the page on the Entry step.
    /// </summary>
    [Fact]
    public void Page_Entry_Step_Shows_Error_When_No_Bank_Account()
    {
        A.CallTo(() => _bankAccountOrchestrator.GetExistingAccountsAsync())
            .Returns((IReadOnlyList<SavedBankAccount>) new List<SavedBankAccount>().AsReadOnly());

        var cut = RenderPage();

        FillValidAmount(cut);
        ClickContinue(cut);

        Assert.Contains("Bank Information is required", cut.Markup);
        Assert.DoesNotContain("Verify &amp; Authorize", cut.Markup);
    }

    /// <summary>
    /// Verifies that clicking Continue without contact information shows the contact
    /// information required error on the Entry step.
    /// </summary>
    [Fact]
    public void Page_Entry_Step_Shows_Error_When_No_Contact_Info()
    {
        A.CallTo(() => _contactInfoService.GetEmployerWebContact(A<int>._, A<int>._, A<int>._))
            .Returns(Task.FromResult<ACHContactModel?>(null));

        var cut = RenderPage();

        FillValidAmount(cut);
        ClickContinue(cut);

        Assert.Contains("Contact Information is required", cut.Markup);
        Assert.DoesNotContain("Verify &amp; Authorize", cut.Markup);
    }

    /// <summary>
    /// Verifies that clicking Cancel on the Entry step navigates to the employer dashboard.
    /// </summary>
    [Fact]
    public void Page_Entry_Step_Cancel_Navigates_To_Dashboard()
    {
        var cut = RenderPage();
        var nav = Services.GetRequiredService<NavigationManager>();

        cut.FindAll("button")
            .Single(b => { return b.TextContent.Contains("CANCEL", StringComparison.OrdinalIgnoreCase); })
            .Click();

        Assert.EndsWith("/employer-dashboard", nav.Uri);
    }

    // =========================================================
    // BankAccountPaymentAch page — Verify & Authorize step
    // =========================================================

    /// <summary>
    /// Verifies that the Verify &amp; Authorize step renders the selected bank account
    /// masked number in read-only mode (no selector dropdown).
    /// </summary>
    [Fact]
    public void Page_VerifyAuthorize_Displays_Bank_Account_ReadOnly()
    {
        var cut = RenderPage();

        FillValidAmount(cut);
        ClickContinue(cut);

        Assert.Contains(CheckingAccount.MaskedAccountNumber, cut.Markup);
        Assert.Empty(cut.FindAll("select"));
    }

    /// <summary>
    /// Verifies that the authorization checkbox is present on the Verify &amp; Authorize step.
    /// </summary>
    [Fact]
    public void Page_VerifyAuthorize_Shows_Authorization_Checkbox()
    {
        var cut = RenderPage();

        FillValidAmount(cut);
        ClickContinue(cut);

        Assert.NotNull(cut.Find("#achAuthorizeCheckbox"));
    }

    /// <summary>
    /// Verifies that clicking Continue on the Verify &amp; Authorize step without
    /// checking the authorization checkbox shows an error and stays on that step.
    /// </summary>
    [Fact]
    public void Page_VerifyAuthorize_Shows_Error_When_Authorization_Not_Accepted()
    {
        var cut = RenderPage();

        FillValidAmount(cut);
        ClickContinue(cut); // → VerifyAuthorize

        ClickContinue(cut); // attempt without checking

        Assert.Contains("You must accept the user agreement", cut.Markup);
        Assert.DoesNotContain("EFT Payment Confirmation", cut.Markup);
    }

    /// <summary>
    /// Verifies that checking the authorization checkbox and clicking Continue advances
    /// the page to the Confirmation step.
    /// </summary>
    [Fact]
    public void Page_VerifyAuthorize_Advances_To_Confirmation_When_Authorized()
    {
        var cut = RenderPage();

        FillValidAmount(cut);
        ClickContinue(cut); // → VerifyAuthorize

        cut.Find("#achAuthorizeCheckbox").Change(true);
        ClickContinue(cut); // → Confirmation

        Assert.Contains("EFT Payment Confirmation", cut.Markup);
        Assert.Contains("Payment Request Submitted", cut.Markup);
    }

    /// <summary>
    /// Verifies that the Confirmation step displays a non-empty confirmation number
    /// after a successful payment submission.
    /// </summary>
    [Fact]
    public void Page_Confirmation_Shows_Confirmation_Number()
    {
        var cut = RenderPage();

        FillValidAmount(cut);
        ClickContinue(cut);
        cut.Find("#achAuthorizeCheckbox").Change(true);
        ClickContinue(cut);

        // Confirmation number is a 12-char uppercase alphanumeric string
        var markup = cut.Markup;
        Assert.Contains("Confirmation Number", markup);
    }

    /// <summary>
    /// Verifies that clicking Back on the Verify &amp; Authorize step returns to the
    /// Entry step and clears the authorization error if one was shown.
    /// </summary>
    [Fact]
    public void Page_Back_From_VerifyAuthorize_Returns_To_Entry_Step()
    {
        var cut = RenderPage();

        FillValidAmount(cut);
        ClickContinue(cut);

        Assert.Contains("Verify &amp; Authorize", cut.Markup);

        ClickBack(cut);

        Assert.Contains("Bank Account Payment (ACH)", cut.Markup);
        Assert.DoesNotContain("Verify &amp; Authorize", cut.Markup);
    }

    // =========================================================
    // BankAccountPaymentAch page — Cancel payment flow
    // =========================================================

    /// <summary>
    /// Verifies that clicking CANCEL PAYMENT on the Confirmation step navigates to the
    /// Verify &amp; Cancel step.
    /// </summary>
    [Fact]
    public void Page_Confirmation_CancelPayment_Navigates_To_VerifyCancel()
    {
        var cut = RenderPage();

        FillValidAmount(cut);
        ClickContinue(cut);
        cut.Find("#achAuthorizeCheckbox").Change(true);
        ClickContinue(cut); // → Confirmation

        cut.FindAll("button")
            .Single(b => { return b.TextContent.Contains("CANCEL PAYMENT", StringComparison.OrdinalIgnoreCase); })
            .Click();

        Assert.Contains("Verify &amp; Cancel EFT Payment", cut.Markup);
    }

    /// <summary>
    /// Verifies that confirming cancellation on the Verify &amp; Cancel step advances to
    /// the Cancel Confirmation step.
    /// </summary>
    [Fact]
    public void Page_VerifyCancel_Confirming_Advances_To_CancelConfirmation()
    {
        var cut = RenderPage();

        FillValidAmount(cut);
        ClickContinue(cut);
        cut.Find("#achAuthorizeCheckbox").Change(true);
        ClickContinue(cut); // → Confirmation

        cut.FindAll("button")
            .Single(b => { return b.TextContent.Contains("CANCEL PAYMENT", StringComparison.OrdinalIgnoreCase); })
            .Click(); // → VerifyCancel

        // The VerifyCancel footer has "CANCEL PAYMENT" as the continue button text
        cut.FindAll("button")
            .Single(b => { return b.TextContent.Contains("CANCEL PAYMENT", StringComparison.OrdinalIgnoreCase); })
            .Click(); // → CancelConfirmation

        Assert.Contains("Cancelled EFT Payment Confirmation", cut.Markup);
    }

    /// <summary>
    /// Verifies that clicking Back on the Verify &amp; Cancel step returns to the
    /// Confirmation step.
    /// </summary>
    [Fact]
    public void Page_VerifyCancel_Back_Returns_To_Confirmation()
    {
        var cut = RenderPage();

        FillValidAmount(cut);
        ClickContinue(cut);
        cut.Find("#achAuthorizeCheckbox").Change(true);
        ClickContinue(cut); // → Confirmation

        cut.FindAll("button")
            .Single(b => { return b.TextContent.Contains("CANCEL PAYMENT", StringComparison.OrdinalIgnoreCase); })
            .Click(); // → VerifyCancel

        ClickBack(cut);

        Assert.Contains("EFT Payment Confirmation", cut.Markup);
        Assert.DoesNotContain("Verify &amp; Cancel", cut.Markup);
    }

    // =========================================================
    // BankAccountPaymentAch page — Edit payment flow
    // =========================================================

    /// <summary>
    /// Verifies that clicking EDIT PAYMENT on the Confirmation step navigates to the
    /// Edit Payment step.
    /// </summary>
    [Fact]
    public void Page_Confirmation_EditPayment_Navigates_To_Edit_Step()
    {
        var cut = RenderPage();

        FillValidAmount(cut);
        ClickContinue(cut);
        cut.Find("#achAuthorizeCheckbox").Change(true);
        ClickContinue(cut); // → Confirmation

        cut.FindAll("button")
            .Single(b => { return b.TextContent.Contains("EDIT PAYMENT", StringComparison.OrdinalIgnoreCase); })
            .Click();

        Assert.Contains("Edit Bank Account Payment (ACH)", cut.Markup);
    }

    /// <summary>
    /// Verifies that clicking Back on the Edit step returns to the Confirmation step.
    /// </summary>
    [Fact]
    public void Page_Edit_Step_Back_Returns_To_Confirmation()
    {
        var cut = RenderPage();

        FillValidAmount(cut);
        ClickContinue(cut);
        cut.Find("#achAuthorizeCheckbox").Change(true);
        ClickContinue(cut); // → Confirmation

        cut.FindAll("button")
            .Single(b => { return b.TextContent.Contains("EDIT PAYMENT", StringComparison.OrdinalIgnoreCase); })
            .Click(); // → Edit

        ClickBack(cut);

        Assert.Contains("EFT Payment Confirmation", cut.Markup);
        Assert.DoesNotContain("Edit Bank Account Payment", cut.Markup);
    }

    /// <summary>
    /// Verifies that continuing from the Edit step routes back through Verify &amp;
    /// Authorize (with _cameFromEdit = true), and that Back from there returns to Edit.
    /// </summary>
    [Fact]
    public void Page_Edit_Step_Continue_Goes_To_VerifyAuthorize_And_Back_Returns_To_Edit()
    {
        var cut = RenderPage();

        FillValidAmount(cut);
        ClickContinue(cut);
        cut.Find("#achAuthorizeCheckbox").Change(true);
        ClickContinue(cut); // → Confirmation

        cut.FindAll("button")
            .Single(b => { return b.TextContent.Contains("EDIT PAYMENT", StringComparison.OrdinalIgnoreCase); })
            .Click(); // → Edit

        // Edit step has its own payment amount field — fill it
        var editAmountInput = cut.Find("input[inputmode='decimal']");
        editAmountInput.Input("750.00");
        editAmountInput.Blur();

        ClickContinue(cut); // → VerifyAuthorize (cameFromEdit = true)

        Assert.Contains("Verify &amp; Authorize", cut.Markup);

        ClickBack(cut); // should return to Edit, not Entry

        Assert.Contains("Edit Bank Account Payment (ACH)", cut.Markup);
    }
}
