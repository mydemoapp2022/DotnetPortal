using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using UI.EmployerPortal.Web.Features.BillingPayments.Components;
using UI.EmployerPortal.Web.Features.BillingPayments.Models;
using UI.EmployerPortal.Web.Features.BillingPayments.Pages;
using UI.EmployerPortal.Web.Features.BillingPayments.Services;
using Xunit;

namespace Test.UI.EmployerPortal.Web;

/// <summary>
/// Component tests for ACH bank account display and selection behaviour,
/// covering both the <see cref="AchBankInfoDisplay"/> component and the
/// <see cref="BankAccountPaymentAch"/> page entry-step validation.
/// </summary>      
public class AchBankAccountTests : BunitContext
{
    // ── Shared stub data ────────────────────────────────────────────────

    private static readonly AchBankAccount CheckingAccount = new()
    {
        AccountNickname    = "Janice's Checking Account",
        AccountHolderName  = "Janice Smith",
        BankName           = "Hometown Bank",
        RoutingNumber      = "015708055",
        MaskedAccountNumber = "********9177",
        AccountType        = "Checking"
    };

    private static readonly AchBankAccount SavingsAccount = new()
    {
        AccountNickname    = "Janice's Savings Account",
        AccountHolderName  = "Janice Smith",
        BankName           = "Hometown Bank",
        RoutingNumber      = "015708055",
        MaskedAccountNumber = "********4321",
        AccountType        = "Savings"
    };

    private static readonly List<AchBankAccount> TwoAccounts = [CheckingAccount, SavingsAccount];

    // ── Constructor ─────────────────────────────────────────────────────

    public AchBankAccountTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;

        var holidayService = A.Fake<IFederalReserveHolidayService>();
        A.CallTo(() => holidayService.IsFederalReserveHoliday(A<DateOnly>._)).Returns(false);
        A.CallTo(() => holidayService.GetInvalidDatesInRange(A<DateOnly>._, A<DateOnly>._))
         .Returns(Array.Empty<DateOnly>());

        Services.AddSingleton(holidayService);
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    private IRenderedComponent<AchBankInfoDisplay> RenderDisplay(
        List<AchBankAccount>? accounts = null,
        AchBankAccount? selected = null,
        bool readOnly = false,
        bool? showEditLink = null)
    {
        return Render<AchBankInfoDisplay>(p =>
        {
            p.Add(x => x.BankAccounts, accounts ?? TwoAccounts);
            p.Add(x => x.BankAccount,  selected ?? accounts?.FirstOrDefault() ?? CheckingAccount);
            p.Add(x => x.ReadOnly,     readOnly);
            if (showEditLink.HasValue)
                p.Add(x => x.ShowEditLink, showEditLink.Value);
        });
    }

    private IRenderedComponent<BankAccountPaymentAch> RenderPage()
    {
        return Render<BankAccountPaymentAch>();
    }

    private static void ClickContinue(IRenderedComponent<BankAccountPaymentAch> cut)
    {
        cut.FindAll("button")
           .Single(b => { return b.TextContent.Contains("CONTINUE", StringComparison.OrdinalIgnoreCase); })
           .Click();
    }

    // ── AchBankInfoDisplay tests ─────────────────────────────────────────

    /// <summary>
    /// Verifies that the first bank account is pre-selected and its details
    /// (nickname, bank name, masked number, account type) are rendered.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Shows_First_Account_Details_By_Default()
    {
        var cut = RenderDisplay();

        Assert.Contains(CheckingAccount.AccountHolderName, cut.Markup);
        Assert.Contains(CheckingAccount.BankName,          cut.Markup);
        Assert.Contains(CheckingAccount.MaskedAccountNumber, cut.Markup);
        Assert.Contains(CheckingAccount.AccountType,       cut.Markup);
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
    /// Verifies that the account selector dropdown is rendered when not in read-only mode
    /// and multiple accounts are available.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Shows_Account_Selector_When_Not_ReadOnly()
    {
        var cut = RenderDisplay(readOnly: false);

        // OutlinedSelectField renders a <select> element
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
    /// displayed bank details to match the newly selected account.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Changing_Account_Updates_Displayed_Details()
    {
        var cut = RenderDisplay(readOnly: false);

        var select = cut.Find("select");
        select.Change(SavingsAccount.AccountNickname);

        Assert.Contains(SavingsAccount.MaskedAccountNumber, cut.Markup);
        Assert.Contains(SavingsAccount.AccountType,         cut.Markup);
    }

    /// <summary>
    /// Verifies that the <see cref="AchBankInfoDisplay.SelectedAccountChanged"/> callback
    /// is invoked with the correct account when the user changes the selection.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Fires_SelectedAccountChanged_Callback_On_Change()
    {
        AchBankAccount? received = null;

        var cut = Render<AchBankInfoDisplay>(p =>
        {
            p.Add(x => x.BankAccounts,            TwoAccounts);
            p.Add(x => x.BankAccount,             CheckingAccount);
            p.Add(x => x.ReadOnly,                false);
            p.Add(x => x.SelectedAccountChanged,  EventCallback.Factory.Create<AchBankAccount>(
                this, account => { received = account; }));
        });

        cut.Find("select").Change(SavingsAccount.AccountNickname);

        Assert.NotNull(received);
        Assert.Equal(SavingsAccount.AccountNickname, received!.AccountNickname);
    }

    /// <summary>
    /// Verifies that the Edit link is rendered when the component is not in read-only mode
    /// and an account is selected.
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
            p.Add(x => x.BankAccounts, new List<AchBankAccount>());
            p.Add(x => x.ReadOnly,     false);
        });

        Assert.Contains("+ Add Account", cut.Markup);
    }

    /// <summary>
    /// Verifies that the "Add Account" link is hidden when no accounts exist but
    /// the component is in read-only mode.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Hides_Add_Account_Link_When_ReadOnly()
    {
        var cut = Render<AchBankInfoDisplay>(p =>
        {
            p.Add(x => x.BankAccounts, new List<AchBankAccount>());
            p.Add(x => x.ReadOnly,     true);
        });

        Assert.DoesNotContain("+ Add Account", cut.Markup);
    }

    /// <summary>
    /// Verifies that the bank information section collapses and the account details
    /// are no longer visible after the collapse toggle button is clicked.
    /// </summary>
    [Fact]
    public void BankInfoDisplay_Collapses_When_Toggle_Button_Clicked()
    {
        var cut = RenderDisplay();

        // Initially expanded — details visible
        Assert.Contains(CheckingAccount.BankName, cut.Markup);

        cut.Find("button.ach-collapse-btn").Click();

        // Collapsed — the region's hidden attribute is set; details no longer visible
        var body = cut.Find("#bank-info-body");
        Assert.True(body.HasAttribute("hidden"));
    }

    // ── BankAccountPaymentAch page entry-step validation tests ───────────

    /// <summary>
    /// Verifies that clicking Continue on the Entry step without any bank account
    /// (simulated by using the page defaults which always pre-populate stub data)
    /// advances to the Verify &amp; Authorize step when the payment info is valid.
    /// This acts as a smoke test for the happy path with pre-populated stub accounts.
    /// </summary>
    [Fact]
    public void Page_Entry_Step_Advances_To_VerifyAuthorize_When_All_Data_Valid()
    {
        var cut = RenderPage();

        // Page pre-populates stub data; heading must reflect Entry step first
        Assert.Contains("Bank Account Payment (ACH)", cut.Markup);

        // Fill a valid payment amount before continuing
        var amountInput = cut.Find("input[inputmode='decimal']");
        amountInput.Input("500.00");
        amountInput.Blur();

        ClickContinue(cut);

        Assert.Contains("Verify &amp; Authorize Bank Account Payment (ACH)", cut.Markup);
    }

    /// <summary>
    /// Verifies that the Verify &amp; Authorize step displays the selected bank account's
    /// masked account number in read-only mode.
    /// </summary>
    [Fact]
    public void Page_VerifyAuthorize_Step_Displays_Bank_Account_In_ReadOnly()
    {
        var cut = RenderPage();

        var amountInput = cut.Find("input[inputmode='decimal']");
        amountInput.Input("100.00");
        amountInput.Blur();

        ClickContinue(cut);

        // Masked number from stub data
        Assert.Contains("********9177", cut.Markup);
        // Selector dropdown must not appear on the verify step
        Assert.Empty(cut.FindAll("select"));
    }

    /// <summary>
    /// Verifies that clicking Back on the Verify &amp; Authorize step returns to the Entry step.
    /// </summary>
    [Fact]
    public void Page_Back_From_VerifyAuthorize_Returns_To_Entry_Step()
    {
        var cut = RenderPage();

        var amountInput = cut.Find("input[inputmode='decimal']");
        amountInput.Input("250.00");
        amountInput.Blur();

        ClickContinue(cut);

        Assert.Contains("Verify &amp; Authorize", cut.Markup);

        cut.FindAll("button")
           .Single(b => { return b.TextContent.Contains("BACK", StringComparison.OrdinalIgnoreCase); })
           .Click();

        Assert.Contains("Bank Account Payment (ACH)", cut.Markup);
        Assert.DoesNotContain("Verify &amp; Authorize", cut.Markup);
    }
}
