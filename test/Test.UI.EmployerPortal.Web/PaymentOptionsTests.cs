using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using UI.EmployerPortal.Web.Features.BillingPayments.Components;
using UI.EmployerPortal.Web.Features.BillingPayments.Models;
using UI.EmployerPortal.Web.Features.BillingPayments.Pages;

namespace Test.UI.EmployerPortal.Web;

public class PaymentOptionsTests : BunitContext
{
    public PaymentOptionsTests()
    {
    }

    private IRenderedComponent<BillingPayments> RenderComponent()
    {
        return Render<BillingPayments>();
    }

    [Fact]
    public void Renders_Page_Title()
    {
        var cut = RenderComponent();
        Assert.Contains("Payment Options", cut.Markup);
    }

    [Fact]
    public void Error_Banner_Not_Visible_By_Default()
    {
        var cut = RenderComponent();
        Assert.DoesNotContain("Payment option is required.", cut.Markup);
    }

    [Fact]
    public async Task Validate_Returns_False_When_No_Option_Selected()
    {
        var cut = RenderComponent();
        var result = await cut.InvokeAsync(cut.Instance.Validate);
        Assert.False(result);
    }

    [Fact]
    public async Task Validate_Shows_Required_Error_When_No_Option_Selected()
    {
        var cut = RenderComponent();
        await cut.InvokeAsync(cut.Instance.Validate);
        Assert.Contains("Payment option is required.", cut.Markup);
    }

    [Fact]
    public async Task Validate_Returns_True_When_Any_Option_Selected()
    {
        var cut = RenderComponent();
        var radios = cut.FindAll("input[type='radio']");
        Assert.NotEmpty(radios);

        radios[0].Change(true);

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.True(result);
    }

    [Fact]
    public async Task Validate_Clears_Error_After_Selecting_An_Option()
    {
        var cut = RenderComponent();

        await cut.InvokeAsync(cut.Instance.Validate);
        Assert.Contains("Payment option is required.", cut.Markup);

        var radios = cut.FindAll("input[type='radio']");
        Assert.NotEmpty(radios);

        radios[0].Change(true);

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.True(result);
        Assert.DoesNotContain("Payment option is required.", cut.Markup);
    }
}
