using System.Reflection;
using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using UI.EmployerPortal.Web.Features.BillingPayments.Components;
using UI.EmployerPortal.Web.Features.BillingPayments.Models;
using UI.EmployerPortal.Web.Features.BillingPayments.Pages;
using Xunit;

namespace Test.UI.EmployerPortal.Web;

public class PaymentOptionsTests : BunitContext
{
    public PaymentOptionsTests()
    {
    }

    private IRenderedComponent<PaymentOptions> RenderComponent()
    {
        return Render<PaymentOptions>();
    }

    private static async Task<bool> InvokeValidateAsync(object instance)
    {
        var method = instance.GetType().GetMethod(
            "Validate",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.NotNull(method);

        var result = method!.Invoke(instance, null);

        return result switch
        {
            Task<bool> task => await task,
            bool value => value,
            _ => throw new InvalidOperationException("Validate must return bool or Task<bool>.")
        };
    }

    [Fact]
    public void Renders_Billing_And_Payments_Page()
    {
        var cut = RenderComponent();

        Assert.Contains("Billing & Payments", cut.Markup);
    }

    [Fact]
    public void Renders_Billing_Component()
    {
        var cut = RenderComponent();

        Assert.NotEmpty(cut.FindAll("div.section-page"));
    }

    [Fact]
    public void Accepts_Source_Query_Parameter_Flow()
    {
        var cut = RenderComponent("flow");

        Assert.Contains("Billing & Payments", cut.Markup);
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

        var result = await cut.InvokeAsync(() => InvokeValidateAsync(cut.Instance));

        Assert.False(result);
    }

    [Fact]
    public async Task Validate_Shows_Required_Error_When_No_Option_Selected()
    {
        var cut = RenderComponent();

        await cut.InvokeAsync(() => InvokeValidateAsync(cut.Instance));

        Assert.Contains(RequiredMessage, cut.Markup);
    }

    [Fact]
    public async Task Validate_Returns_True_When_Any_Option_Selected()
    {
        var cut = RenderComponent();

        var radios = cut.FindAll("input[type='radio']");
        Assert.NotEmpty(radios);

        radios[0].Change(true);

        var result = await cut.InvokeAsync(() => InvokeValidateAsync(cut.Instance));

        Assert.True(result);
    }

    [Fact]
    public async Task Validate_Clears_Error_After_Selecting_An_Option()
    {
        var cut = RenderComponent();

        await cut.InvokeAsync(() => InvokeValidateAsync(cut.Instance));
        Assert.Contains(RequiredMessage, cut.Markup);

        var radios = cut.FindAll("input[type='radio']");
        Assert.NotEmpty(radios);

        radios[0].Change(true);

        var result = await cut.InvokeAsync(() => InvokeValidateAsync(cut.Instance));

        Assert.True(result);
        Assert.DoesNotContain(RequiredMessage, cut.Markup);
    }
}
