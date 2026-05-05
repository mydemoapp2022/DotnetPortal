using System.Reflection;
using Bunit;
using FakeItEasy;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using UI.EmployerPortal.Web.Features.BillingPayments.Components;
using UI.EmployerPortal.Web.Features.BillingPayments.Models;
using UI.EmployerPortal.Web.Features.BillingPayments.Pages;
using Xunit;

namespace Test.UI.EmployerPortal.Web;

public class PaymentOptionsTests : BunitContext
{
    private const string RequiredMessage = "Please select a payment option to continue.";

    private IRenderedComponent<BillingPaymentsMain> RenderComponent()
    {
        return Render<BillingPaymentsMain>(p =>
        {
            p.Add(x => x.Source, NavigationSource.Menu);
        });
    }

    private static void ClickMakePayment(IRenderedComponent<BillingPaymentsMain> cut)
    {
        // Robust selector: relies on visible button text instead of CSS class only.
        var continueButton = cut.FindAll("button")
            .Single(b => b.TextContent.Contains("MAKE A PAYMENT", StringComparison.OrdinalIgnoreCase));

        continueButton.Click();
    }

    [Fact]
    public void Validate_Returns_False_When_No_Option_Selected()
    {
        var cut = RenderComponent();

        ClickMakePayment(cut);

        Assert.Contains(RequiredMessage, cut.Markup);
    }

    [Fact]
    public void Validate_Shows_Required_Error_When_No_Option_Selected()
    {
        var cut = RenderComponent();

        ClickMakePayment(cut);

        Assert.Contains(RequiredMessage, cut.Markup);
    }

    [Fact]
    public void Validate_Returns_True_When_Any_Option_Selected()
    {
        var cut = RenderComponent();
        var nav = Services.GetRequiredService<NavigationManager>();

        cut.FindAll("div[role='radio']")[0].Click(); // ACH
        ClickMakePayment(cut);

        Assert.EndsWith("/billing-payments/payment-history", nav.Uri);
    }

    [Fact]
    public void Validate_Clears_Error_After_Selecting_An_Option()
    {
        var cut = RenderComponent();
        var nav = Services.GetRequiredService<NavigationManager>();

        ClickMakePayment(cut);
        Assert.Contains(RequiredMessage, cut.Markup);

        cut.FindAll("div[role='radio']")[1].Click(); // Card
        ClickMakePayment(cut);

        Assert.DoesNotContain(RequiredMessage, cut.Markup);
        Assert.EndsWith("/billing-payments/payment-history", nav.Uri);
    }
}
