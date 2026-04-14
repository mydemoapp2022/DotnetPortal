using Bunit;
using FakeItEasy;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using UI.EmployerPortal.Generated.ServiceClients.EmployerRegistrationService;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Pages;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Services;
using UI.EmployerPortal.Web.Features.Shared.Registrations.Models;
namespace Test.UI.EmployerPortal.Web.Component.Features.EmployerRegistration;

public class EmployerRegEntryPointTests : BunitContext
{
    private readonly IEmployerRegistrationServices _mockService;
    public EmployerRegEntryPointTests()
    {
        _mockService = A.Fake<IEmployerRegistrationServices>();
        Services.AddSingleton(_mockService);
        Services.AddSingleton(new EmployerRegistrationInfo());
    }
    // Render Tests
    [Fact]
    public void Renders_Page_Title()
    {
        var cut = Render<EmployerRegEntryPoint>();
        Assert.Equal("New Employer Registration",
            cut.Find("h1.page-title").TextContent.Trim());
    }
    [Fact]
    public void No_Error_Banner_Before_Submit()
    {
        var cut = Render<EmployerRegEntryPoint>();
        Assert.Empty(cut.FindAll(".bi-error-banner"));
    }
    // Validation Tests
    [Fact]
    public void Submit_Without_Selecting_Radio_Shows_Missing_Information_Banner()
    {
        var cut = Render<EmployerRegEntryPoint>();
        cut.Find("button.btn-outline-primary").Click();
        Assert.Contains("Missing information", cut.Markup);
    }
    [Fact]
    public void Submit_Without_Selecting_Radio_Shows_Required_Error()
    {
        var cut = Render<EmployerRegEntryPoint>();
        cut.Find("button.btn-outline-primary").Click();
        Assert.Contains("Select whether you have a registration number", cut.Markup);
    }
    [Fact]
    public void Yes_Selected_Empty_Fields_Shows_Required_Errors()
    {
        // Set it on shared model
        var registrationInfo = Services.GetRequiredService<EmployerRegistrationInfo>();
        registrationInfo.HasRegistrationNumber = true;

        var cut = Render<EmployerRegEntryPoint>();
        cut.Find("button.btn-outline-primary").Click();
        Assert.Contains("Registration Number is required if you answer Yes", cut.Markup);
        Assert.Contains("FEIN is required if you answer Yes", cut.Markup);
    }
    [Fact]
    public void Yes_Selected_Only_RegistrationNumber_Filled_Shows_FEIN_Error()
    {
        // Set it on shared model
        var registrationInfo = Services.GetRequiredService<EmployerRegistrationInfo>();
        registrationInfo.HasRegistrationNumber = true;

        var cut = Render<EmployerRegEntryPoint>();
        cut.Find("input[name*='RegistrationNumber']").Change("12345678");
        cut.Find("button.btn-outline-primary").Click();
        Assert.Contains("FEIN is required if you answer Yes", cut.Markup);
    }
    // WCF Error Tests
    [Fact]
    public void Valid_Fields_WCF_Fails_Shows_Inline_Error_Message()
    {

        // Set it on shared model
        var registrationInfo = Services.GetRequiredService<EmployerRegistrationInfo>();
        registrationInfo.HasRegistrationNumber = true;

        var cut = Render<EmployerRegEntryPoint>();
        cut.Find("input[aria-label*='Registration Number']").Input("12345678");
        cut.Find("input[aria-label*='Federal Employer Identification Number']").Input("12-3123456");

        A.CallTo(() => _mockService.ContinueRegistration(
            A<string>._, A<string>._))
            .Returns(new PortalContinueRegistrationResponse
            {
                SurveyResponseSK = null,
                Message = "The registration number and the FEIN entered was not found"
            });
        cut.Find("button.btn-outline-primary").Click();

        Assert.Contains(
            "The registration number and the FEIN entered was not found",
            cut.Markup);
    }
    // Navigation Tests
    [Fact]
    public void No_Selected_Next_Navigates_To_Welcome()
    {
        // Set it on shared model
        var registrationInfo = Services.GetRequiredService<EmployerRegistrationInfo>();
        registrationInfo.HasRegistrationNumber = false;

        var cut = Render<EmployerRegEntryPoint>();
        cut.Find("button.btn-outline-primary").Click();

        var nav = Services.GetRequiredService<NavigationManager>();
        Assert.Contains("employer-registration-welcome", nav.Uri);
    }
    [Fact]
    public void Valid_Fields_WCF_Succeeds_Navigates_To_Welcome()
    {
        // Set it on shared model
        var registrationInfo = Services.GetRequiredService<EmployerRegistrationInfo>();
        registrationInfo.HasRegistrationNumber = true;

        var cut = Render<EmployerRegEntryPoint>();
        cut.Find("input[aria-label*='Registration Number']").Input("12345678");
        cut.Find("input[aria-label*='Federal Employer Identification Number']").Input("12-3123456");

        A.CallTo(() => _mockService.ContinueRegistration(
            A<string>._, A<string>._))
            .Returns(new PortalContinueRegistrationResponse { SurveyResponseSK = 99999 });

        cut.Find("button.btn-outline-primary").Click();

        var nav = Services.GetRequiredService<NavigationManager>();
        Assert.Contains("employer-registration-welcome", nav.Uri);

    }

    [Fact]
    public void Back_Button_Navigates_Back()
    {
        var cut = Render<EmployerRegEntryPoint>();
        cut.Find("button.btn-outline-secondary").Click();
        var nav = Services.GetRequiredService<NavigationManager>();
        Assert.Contains("/", nav.Uri);
    }
}
