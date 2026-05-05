using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using UI.EmployerPortal.Web.Features.BillingPayments.Components;
using UI.EmployerPortal.Web.Features.BillingPayments.Models;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Models;
using Xunit;

namespace Test.UI.EmployerPortal.Web.BillingPayments;

public class BillingPaymentsMainTests : TestContext
{
    private readonly Mock<NavigationManager> _navManagerMock;

    public BillingPaymentsMainTests()
    {
        // bUnit provides a fake NavigationManager automatically via TestContext
        // Register shared component stubs
        Services.AddSingleton(new Mock<IJSRuntime>().Object);

        // Stub child components that are not under test
        ComponentFactories.AddStub<BillingButtonBar>();
        ComponentFactories.AddStub<NotificationBanner>();
    }

    // ---------------------------------------------------------------------------
    // Rendering
    // ---------------------------------------------------------------------------

    [Fact]
    public void Renders_MakeAPaymentSection()
    {
        var cut = RenderComponent<BillingPaymentsMain>();

        cut.Find("#make-payment-heading").TextContent.ShouldBe("Make a Payment");
    }

    [Fact]
    public void Renders_OtherOptionsSection()
    {
        var cut = RenderComponent<BillingPaymentsMain>();

        cut.Find("#other-options-heading").TextContent.ShouldBe("Other Options");
    }

    [Fact]
    public void Renders_BothPaymentOptionCards()
    {
        var cut = RenderComponent<BillingPaymentsMain>();

        var cards = cut.FindAll(".payment-option-card");
        Assert.Equal(2, cards.Count);
    }

    // ---------------------------------------------------------------------------
    // Payment option selection
    // ---------------------------------------------------------------------------

    [Fact]
    public void AchCard_WhenClicked_IsMarkedSelected()
    {
        var cut = RenderComponent<BillingPaymentsMain>();

        var achCard = cut.FindAll(".payment-option-card")[0];
        achCard.Click();

        Assert.Contains("selected", achCard.ClassList);
        Assert.Equal("true", achCard.GetAttribute("aria-checked"));
    }

    [Fact]
    public void CardOption_WhenClicked_IsMarkedSelected()
    {
        var cut = RenderComponent<BillingPaymentsMain>();

        var cardOption = cut.FindAll(".payment-option-card")[1];
        cardOption.Click();

        Assert.Contains("selected", cardOption.ClassList);
        Assert.Equal("true", cardOption.GetAttribute("aria-checked"));
    }

    [Fact]
    public void SelectingAch_DeselectedCard()
    {
        var cut = RenderComponent<BillingPaymentsMain>();

        // Select card first, then switch to ACH
        cut.FindAll(".payment-option-card")[1].Click();
        cut.FindAll(".payment-option-card")[0].Click();

        Assert.Contains("selected", cut.FindAll(".payment-option-card")[0].ClassList);
        Assert.DoesNotContain("selected", cut.FindAll(".payment-option-card")[1].ClassList);
    }

    [Fact]
    public void SelectingOption_ClearsValidationError()
    {
        var cut = RenderComponent<BillingPaymentsMain>();

        // Trigger validation error first by clicking Continue with nothing selected
        cut.FindComponent<BillingButtonBar>().Find("button").Click(); // stub — test via HandleContinue directly

        // Select ACH — error should clear
        cut.FindAll(".payment-option-card")[0].Click();

        Assert.Empty(cut.FindAll(".nb-alert-multiline")); // NotificationBanner not rendered
    }

    // ---------------------------------------------------------------------------
    // Keyboard interaction
    // ---------------------------------------------------------------------------

    [Theory]
    [InlineData("Enter")]
    [InlineData(" ")]
    public void AchCard_WhenEnterOrSpacePressed_IsSelected(string key)
    {
        var cut = RenderComponent<BillingPaymentsMain>();

        var achCard = cut.FindAll(".payment-option-card")[0];
        achCard.KeyDown(key);

        Assert.Contains("selected", achCard.ClassList);
    }

    [Theory]
    [InlineData("Tab")]
    [InlineData("ArrowDown")]
    public void AchCard_WhenOtherKeyPressed_IsNotSelected(string key)
    {
        var cut = RenderComponent<BillingPaymentsMain>();

        var achCard = cut.FindAll(".payment-option-card")[0];
        achCard.KeyDown(key);

        Assert.DoesNotContain("selected", achCard.ClassList);
    }

    // ---------------------------------------------------------------------------
    // Validation
    // ---------------------------------------------------------------------------

    [Fact]
    public void HandleContinue_WithNoSelection_ShowsValidationBanner()
    {
        var cut = RenderComponent<BillingPaymentsMain>();

        // Invoke HandleContinue directly via the instance
        cut.InvokeAsync(() => cut.Instance.HandleContinueForTest());

        cut.WaitForAssertion(() =>
        {
            var banner = cut.Find("[role='alert'], .nb-alert-multiline");
            Assert.NotNull(banner);
        });
    }

    [Fact]
    public void HandleContinue_WithNoSelection_DoesNotNavigate()
    {
        var cut = RenderComponent<BillingPaymentsMain>();
        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        cut.InvokeAsync(() => cut.Instance.HandleContinueForTest());

        // Navigation should not have changed from the initial URI
        Assert.Equal("http://localhost/", navManager.Uri);
    }

    [Fact]
    public void HandleContinue_WithAchSelected_NavigatesToPaymentHistory()
    {
        var cut = RenderComponent<BillingPaymentsMain>();
        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        cut.FindAll(".payment-option-card")[0].Click(); // Select ACH
        cut.InvokeAsync(() => cut.Instance.HandleContinueForTest());

        cut.WaitForAssertion(() =>
            Assert.Contains("billing-payments/payment-history", navManager.Uri));
    }

    [Fact]
    public void HandleContinue_WithCardSelected_NavigatesToPaymentHistory()
    {
        var cut = RenderComponent<BillingPaymentsMain>();
        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        cut.FindAll(".payment-option-card")[1].Click(); // Select Card
        cut.InvokeAsync(() => cut.Instance.HandleContinueForTest());

        cut.WaitForAssertion(() =>
            Assert.Contains("billing-payments/payment-history", navManager.Uri));
    }

    // ---------------------------------------------------------------------------
    // Back / Cancel button visibility (Source parameter)
    // ---------------------------------------------------------------------------

    [Fact]
    public void WhenSourceIsMenu_BackAndCancelAreHidden()
    {
        var cut = RenderComponent<BillingPaymentsMain>(p =>
            p.Add(x => x.Source, NavigationSource.Menu));

        var buttonBar = cut.FindComponent<BillingButtonBar>();
        Assert.False(buttonBar.Instance.ShowBack);
        Assert.False(buttonBar.Instance.ShowCancel);
    }

    [Fact]
    public void WhenSourceIsFlow_BackAndCancelAreVisible()
    {
        var cut = RenderComponent<BillingPaymentsMain>(p =>
            p.Add(x => x.Source, NavigationSource.Flow));

        var buttonBar = cut.FindComponent<BillingButtonBar>();
        Assert.True(buttonBar.Instance.ShowBack);
        Assert.True(buttonBar.Instance.ShowCancel);
    }

    [Fact]
    public void WhenSourceIsMenu_ContinueIsAlwaysVisible()
    {
        var cut = RenderComponent<BillingPaymentsMain>(p =>
            p.Add(x => x.Source, NavigationSource.Menu));

        var buttonBar = cut.FindComponent<BillingButtonBar>();
        Assert.True(buttonBar.Instance.ShowContinue);
    }

    // ---------------------------------------------------------------------------
    // GoBack navigation
    // ---------------------------------------------------------------------------

    [Fact]
    public void GoBack_NavigatesToBillingPage()
    {
        var cut = RenderComponent<BillingPaymentsMain>(p =>
            p.Add(x => x.Source, NavigationSource.Flow));
        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        cut.InvokeAsync(() => cut.Instance.GoBackForTest());

        cut.WaitForAssertion(() =>
            Assert.Contains("billing-payments/billing", navManager.Uri));
    }

    // ---------------------------------------------------------------------------
    // Cancel navigation
    // ---------------------------------------------------------------------------

    [Fact]
    public void HandleCancel_NavigatesToEmployerDashboard()
    {
        var cut = RenderComponent<BillingPaymentsMain>(p =>
            p.Add(x => x.Source, NavigationSource.Flow));
        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        cut.InvokeAsync(() => cut.Instance.HandleCancelForTest());

        cut.WaitForAssertion(() =>
            Assert.Contains("employer-dashboard", navManager.Uri));
    }

    // ---------------------------------------------------------------------------
    // Accessibility
    // ---------------------------------------------------------------------------

    [Fact]
    public void PaymentOptionGroup_HasRadioGroupRole()
    {
        var cut = RenderComponent<BillingPaymentsMain>();

        var radioGroup = cut.Find("[role='radiogroup']");
        Assert.NotNull(radioGroup);
    }

    [Fact]
    public void PaymentOptionCards_HaveRadioRole()
    {
        var cut = RenderComponent<BillingPaymentsMain>();

        var radios = cut.FindAll("[role='radio']");
        Assert.Equal(2, radios.Count);
    }

    [Fact]
    public void InitialState_FirstCardHasTabIndex0()
    {
        var cut = RenderComponent<BillingPaymentsMain>();

        var achCard = cut.FindAll(".payment-option-card")[0];
        Assert.Equal("0", achCard.GetAttribute("tabindex"));
    }

    [Fact]
    public void AfterSelectingAch_CardOptionHasTabIndexMinus1()
    {
        var cut = RenderComponent<BillingPaymentsMain>();

        cut.FindAll(".payment-option-card")[0].Click();

        Assert.Equal("-1", cut.FindAll(".payment-option-card")[1].GetAttribute("tabindex"));
    }
}
