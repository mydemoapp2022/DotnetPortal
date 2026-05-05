using UI.EmployerPortal.Web.Features.BillingPayments.Models;
using UI.EmployerPortal.Web.Features.Dashboard;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Models;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using UI.EmployerPortal.Web.Features.BillingPayments.Pages;
using Xunit;
using FakeItEasy;

namespace Test.UI.EmployerPortal.Web.BillingPayments;

public class PaymentOptionsPageTests : BunitContext
{
    private readonly IDashboardOrchestrator _orchestratorFake = A.Fake<IDashboardOrchestrator>();

    public PaymentOptionsPageTests()
    {
        A.CallTo(() => _orchestratorFake.GetSelectedEmployerAccountAsync())
            .Returns(new EmployerAccount());

        Services.AddSingleton(_orchestratorFake);

        // Stub child components
        ComponentFactories.AddStub<BillingPaymentsHeader>();
        ComponentFactories.AddStub<BillingPaymentsMain>();
    }

    [Fact]
    public void WhenNoQueryString_SourceIsMenu()
    {
        var navManager = Services.GetRequiredService<FakeNavigationManager>();
        navManager.NavigateTo("/billing-payments/payment-options"); // no ?source

        var cut = RenderComponent<PaymentOptions>();

        var main = cut.FindComponent<BillingPaymentsMain>();
        Assert.Equal(NavigationSource.Menu, main.Instance.Source);
    }

    [Fact]
    public void WhenSourceFlowQueryString_SourceIsFlow()
    {
        var navManager = Services.GetRequiredService<FakeNavigationManager>();
        navManager.NavigateTo("/billing-payments/payment-options?source=flow");

        var cut = RenderComponent<PaymentOptions>();

        var main = cut.FindComponent<BillingPaymentsMain>();
        Assert.Equal(NavigationSource.Flow, main.Instance.Source);
    }

    [Fact]
    public void WhenSourceFlowQueryString_CaseInsensitive()
    {
        var navManager = Services.GetRequiredService<FakeNavigationManager>();
        navManager.NavigateTo("/billing-payments/payment-options?source=FLOW");

        var cut = RenderComponent<PaymentOptions>();

        var main = cut.FindComponent<BillingPaymentsMain>();
        Assert.Equal(NavigationSource.Flow, main.Instance.Source);
    }

    [Fact]
    public void WhenUnknownSource_DefaultsToMenu()
    {
        var navManager = Services.GetRequiredService<FakeNavigationManager>();
        navManager.NavigateTo("/billing-payments/payment-options?source=unknown");

        var cut = RenderComponent<PaymentOptions>();

        var main = cut.FindComponent<BillingPaymentsMain>();
        Assert.Equal(NavigationSource.Menu, main.Instance.Source);
    }
}
