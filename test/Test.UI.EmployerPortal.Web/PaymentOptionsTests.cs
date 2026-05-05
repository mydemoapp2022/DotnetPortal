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
    private const string RequiredMessage = "Payment option is required.";
    private IRenderedComponent<BillingPaymentsMain> RenderComponent()
    {
        return Render<BillingPaymentsMain>(p =>
        {
            p.Add(x => x.Source, NavigationSource.Menu);
        });
    }

    [Fact]
    public void Validate_Returns_False_When_No_Option_Selected()
    {
        var cut = RenderComponent();

        cut.Find("button[type='button'].btn-primary").Click();

        Assert.Contains(RequiredMessage, cut.Markup);
    }

    [Fact]
    public void Validate_Shows_Required_Error_When_No_Option_Selected()
    {
        var cut = RenderComponent();

        cut.Find("button[type='button'].btn-primary").Click();

        Assert.Contains(RequiredMessage, cut.Markup);
    }

    [Fact]
    public void Validate_Returns_True_When_Any_Option_Selected()
    {
        var cut = RenderComponent();
        var nav = Services.GetRequiredService<NavigationManager>();

        cut.FindAll("div[role='radio']")[0].Click();
        cut.Find("button[type='button'].btn-primary").Click();

        Assert.EndsWith("/billing-payments/payment-history", nav.Uri);
    }

    [Fact]
    public void Validate_Clears_Error_After_Selecting_An_Option()
    {
        var cut = RenderComponent();
        var nav = Services.GetRequiredService<NavigationManager>();

        // First attempt without selection -> shows validation
        cut.Find("button[type='button'].btn-primary").Click();
        Assert.Contains(RequiredMessage, cut.Markup);

        // Select option and continue again -> clears validation and navigates
        cut.FindAll("div[role='radio']")[1].Click(); // Select Card
        cut.Find("button[type='button'].btn-primary").Click();

        Assert.DoesNotContain(RequiredMessage, cut.Markup);
        Assert.EndsWith("/billing-payments/payment-history", nav.Uri);
    }
}
