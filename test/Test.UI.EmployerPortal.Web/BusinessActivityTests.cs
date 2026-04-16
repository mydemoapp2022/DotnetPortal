using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using UI.EmployerPortal.Generated.ServiceClients.EmployerRegistrationService;
using UI.EmployerPortal.Web.Features.EmployerRegistration;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Components;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Models;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Services;

namespace Test.UI.EmployerPortal.Web.Component.Features.EmployerRegistration;

/// <summary>
/// Component tests for BusinessActivity.
/// Validation is triggered via Validate() (called by the wizard), not a submit button.
/// </summary>
public class BusinessActivityTests : BunitContext
{
    public BusinessActivityTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton(A.Fake<IEmployerRegistrationService>());
        Services.AddSingleton(A.Fake<IUserAccountService>());
        Services.AddSingleton<EmployerRegistrationModelStore>();
    }

    // =========================================================
    // HELPER METHODS
    // =========================================================

    /// <summary>
    /// Sets HavePaidEmployeesForWorkInWisconsin = true on the store so date fields are shown.
    /// </summary>
    private EmployerRegistrationModelStore WithPaidEmployees()
    {
        var store = Services.GetRequiredService<EmployerRegistrationModelStore>();
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.HavePaidEmployeesForWorkInWisconsin = true;
        return store;
    }

    /// <summary>
    /// Minimum valid model when HavePaidEmployees = false.
    /// Only PrincipalBusinessActivity and PrimaryBusinessActivityDescription are required.
    /// </summary>
    private static BusinessActivityModel MakeValidModelNoDates()
    {
        return new BusinessActivityModel
        {
            PrincipalBusinessActivity = PrincipalBusinessActivityType.Retail,
            PrimaryBusinessActivityDescription = "Retail sales of clothing"
        };
    }

    /// <summary>
    /// Valid model when HavePaidEmployees = true — includes all three date fields.
    /// </summary>
    private static BusinessActivityModel MakeValidModelWithDates()
    {
        return new BusinessActivityModel
        {
            DateBusinessStarted = DateTime.Today.AddYears(-2),
            DateFirstPaidEmployeesInWI = DateTime.Today.AddYears(-1),
            DateFirstPaidWagesInWI = DateTime.Today.AddYears(-1),
            PrincipalBusinessActivity = PrincipalBusinessActivityType.Retail,
            PrimaryBusinessActivityDescription = "Retail sales of clothing"
        };
    }

    /// <summary>
    /// Valid model for EmployerServices activity — both yes/no questions answered "No"
    /// requires an explanation, so answer "Yes" to SuppliesTemporaryWorkers to avoid that branch.
    /// </summary>
    private static BusinessActivityModel MakeValidModelEmployerServices()
    {
        return new BusinessActivityModel
        {
            PrincipalBusinessActivity = PrincipalBusinessActivityType.EmployerServices,
            PrimaryBusinessActivityDescription = "Temporary staffing",
            SuppliesTemporaryWorkers = true,
            ProvidesEmployeeLeasing = false
        };
    }

    /// <summary>
    /// Valid model for EmployerServicesPayrollService activity.
    /// </summary>
    private static BusinessActivityModel MakeValidModelPayrollService()
    {
        return new BusinessActivityModel
        {
            PrincipalBusinessActivity = PrincipalBusinessActivityType.EmployerServicesPayrollService,
            PrimaryBusinessActivityDescription = "Payroll processing",
            EmployeeType = "Client employees",
            EmployeeCount = "10",
            ServicesDescription = "Payroll and HR services"
        };
    }

    // =========================================================
    // RENDERING TESTS
    // =========================================================

    [Fact]
    public void Renders_Page_Title()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel());
        });

        Assert.Equal("Business Activity", cut.Find("h1.page-title").TextContent.Trim());
    }

    [Fact]
    public void Date_Fields_Not_Shown_When_HavePaidEmployees_Is_False()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel());
        });

        Assert.Empty(cut.FindAll("#dateStarted"));
        Assert.Empty(cut.FindAll("#dateFirstPaid"));
        Assert.Empty(cut.FindAll("#dateFirstWages"));
    }

    [Fact]
    public void Date_Fields_Shown_When_HavePaidEmployees_Is_True()
    {
        WithPaidEmployees();

        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel());
        });

        Assert.NotEmpty(cut.FindAll("#dateStarted"));
        Assert.NotEmpty(cut.FindAll("#dateFirstPaid"));
        Assert.NotEmpty(cut.FindAll("#dateFirstWages"));
    }

    [Fact]
    public void Principal_Activity_Dropdown_Rendered()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel());
        });

        Assert.NotEmpty(cut.FindAll("#principal-activity"));
    }

    [Fact]
    public void Primary_Description_Textarea_Rendered()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel());
        });

        Assert.NotEmpty(cut.FindAll("#primaryDescription"));
    }

    [Fact]
    public void No_Error_Banner_Before_Validate()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel());
        });

        Assert.Empty(cut.FindAll(".notification-banner--error"));
    }

    // =========================================================
    // CONSTRUCTION WARNING
    // =========================================================

    [Fact]
    public void Construction_Warning_Not_Shown_For_Non_Construction_Activity()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.Retail
            });
        });

        Assert.DoesNotContain("engaged in a construction industry", cut.Markup);
    }

    [Fact]
    public void Construction_Warning_Shown_For_Construction_Activity()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.ConstructionSpecialtyTrades
            });
        });

        Assert.Contains("engaged in a construction industry", cut.Markup);
    }

    // =========================================================
    // EMPLOYER SERVICES SECTION
    // =========================================================

    [Fact]
    public void Employer_Services_Questions_Not_Shown_For_Non_EmployerServices_Activity()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.Retail
            });
        });

        Assert.Empty(cut.FindAll("#tempWorkers"));
        Assert.Empty(cut.FindAll("#employeeLeasing"));
    }

    [Fact]
    public void Employer_Services_Questions_Shown_For_EmployerServices_Activity()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.EmployerServices
            });
        });

        Assert.NotEmpty(cut.FindAll("#tempWorkers"));
        Assert.NotEmpty(cut.FindAll("#employeeLeasing"));
    }

    [Fact]
    public void Explanation_Field_Shown_When_Both_Employer_Services_Answers_Are_No()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.EmployerServices,
                SuppliesTemporaryWorkers = false,
                ProvidesEmployeeLeasing = false
            });
        });

        Assert.NotEmpty(cut.FindAll("#employerExplanation"));
    }

    [Fact]
    public void Explanation_Field_Not_Shown_When_SuppliesTemporaryWorkers_Is_Yes()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.EmployerServices,
                SuppliesTemporaryWorkers = true,
                ProvidesEmployeeLeasing = false
            });
        });

        Assert.Empty(cut.FindAll("#employerExplanation"));
    }

    // =========================================================
    // PAYROLL SERVICE SECTION
    // =========================================================

    [Fact]
    public void Payroll_Service_Fields_Not_Shown_For_Non_PayrollService_Activity()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.Retail
            });
        });

        Assert.Empty(cut.FindAll("#employeeType"));
        Assert.Empty(cut.FindAll("#servicesDescription"));
    }

    [Fact]
    public void Payroll_Service_Fields_Shown_For_PayrollService_Activity()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.EmployerServicesPayrollService
            });
        });

        Assert.NotEmpty(cut.FindAll("#employeeType"));
        Assert.NotEmpty(cut.FindAll("#servicesDescription"));
    }

    // =========================================================
    // SAME AS PRIMARY CHECKBOX
    // =========================================================

    [Fact]
    public void Wisconsin_Description_Textarea_Shown_When_SameAsPrimary_Is_False()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.Retail,
                SameAsPrimaryBusinessActivity = false
            });
        });

        Assert.NotEmpty(cut.FindAll("#wiDescription"));
    }

    [Fact]
    public void Wisconsin_Description_Textarea_Not_Shown_When_SameAsPrimary_Is_True()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.Retail,
                SameAsPrimaryBusinessActivity = true
            });
        });

        Assert.Empty(cut.FindAll("#wiDescription"));
    }

    // =========================================================
    // VALIDATION TESTS — RETURNS FALSE
    // =========================================================

    [Fact]
    public async Task Validate_Returns_False_When_Model_Is_Empty()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel());
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.False(result);
    }

    [Fact]
    public async Task Validate_Returns_False_When_PrincipalActivity_Is_None()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrimaryBusinessActivityDescription = "Some description"
            });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.False(result);
    }

    [Fact]
    public async Task Validate_Returns_False_When_PrimaryDescription_Is_Empty()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.Retail
            });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.False(result);
    }

    [Fact]
    public async Task Validate_Returns_False_When_DateBusinessStarted_Missing_And_HavePaidEmployees_True()
    {
        WithPaidEmployees();

        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                DateFirstPaidEmployeesInWI = DateTime.Today.AddYears(-1),
                DateFirstPaidWagesInWI = DateTime.Today.AddYears(-1),
                PrincipalBusinessActivity = PrincipalBusinessActivityType.Retail,
                PrimaryBusinessActivityDescription = "Retail"
            });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.False(result);
    }

    [Fact]
    public async Task Validate_Returns_False_When_DateFirstPaidEmployees_Missing_And_HavePaidEmployees_True()
    {
        WithPaidEmployees();

        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                DateBusinessStarted = DateTime.Today.AddYears(-2),
                DateFirstPaidWagesInWI = DateTime.Today.AddYears(-1),
                PrincipalBusinessActivity = PrincipalBusinessActivityType.Retail,
                PrimaryBusinessActivityDescription = "Retail"
            });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.False(result);
    }

    [Fact]
    public async Task Validate_Returns_False_When_DateFirstPaidWages_Missing_And_HavePaidEmployees_True()
    {
        WithPaidEmployees();

        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                DateBusinessStarted = DateTime.Today.AddYears(-2),
                DateFirstPaidEmployeesInWI = DateTime.Today.AddYears(-1),
                PrincipalBusinessActivity = PrincipalBusinessActivityType.Retail,
                PrimaryBusinessActivityDescription = "Retail"
            });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.False(result);
    }

    [Fact]
    public async Task Validate_Returns_False_When_EmployerServices_Questions_Not_Answered()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.EmployerServices,
                PrimaryBusinessActivityDescription = "Staffing"
                // SuppliesTemporaryWorkers and ProvidesEmployeeLeasing both null
            });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.False(result);
    }

    [Fact]
    public async Task Validate_Returns_False_When_EmployerServices_Both_No_And_No_Explanation()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.EmployerServices,
                PrimaryBusinessActivityDescription = "Staffing",
                SuppliesTemporaryWorkers = false,
                ProvidesEmployeeLeasing = false
                // EmployerServiceExplanantion is null
            });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.False(result);
    }

    [Fact]
    public async Task Validate_Returns_False_When_PayrollService_Fields_Missing()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.EmployerServicesPayrollService,
                PrimaryBusinessActivityDescription = "Payroll"
                // EmployeeType, EmployeeCount, ServicesDescription all missing
            });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.False(result);
    }

    // =========================================================
    // VALIDATION TESTS — RETURNS TRUE
    // =========================================================

    [Fact]
    public async Task Validate_Returns_True_When_Model_Valid_And_No_Dates_Required()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, MakeValidModelNoDates());
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.True(result);
    }

    [Fact]
    public async Task Validate_Returns_True_When_Model_Valid_With_Dates()
    {
        WithPaidEmployees();

        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, MakeValidModelWithDates());
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.True(result);
    }

    [Fact]
    public async Task Validate_Returns_True_For_EmployerServices_When_Valid()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, MakeValidModelEmployerServices());
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.True(result);
    }

    [Fact]
    public async Task Validate_Returns_True_For_EmployerServices_Both_No_With_Explanation()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.EmployerServices,
                PrimaryBusinessActivityDescription = "Staffing",
                SuppliesTemporaryWorkers = false,
                ProvidesEmployeeLeasing = false,
                EmployerServiceExplanantion = "We perform internal HR functions only"
            });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.True(result);
    }

    [Fact]
    public async Task Validate_Returns_True_For_PayrollService_When_Valid()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, MakeValidModelPayrollService());
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.True(result);
    }

    // =========================================================
    // VALIDATION ERROR MESSAGE TESTS
    // =========================================================

    [Fact]
    public async Task Validate_Shows_Error_Banner_When_Form_Is_Invalid()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel());
        });

        await cut.InvokeAsync(cut.Instance.Validate);

        Assert.NotEmpty(cut.FindAll(".notification-banner--error"));
    }

    [Fact]
    public async Task Validate_Shows_PrincipalActivity_Error_When_Not_Selected()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel());
        });

        await cut.InvokeAsync(cut.Instance.Validate);

        Assert.Contains("Principal Business Activity is required", cut.Markup);
    }

    [Fact]
    public async Task Validate_Shows_PrimaryDescription_Error_When_Empty()
    {
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.Retail
            });
        });

        await cut.InvokeAsync(cut.Instance.Validate);

        Assert.Contains("Primary Business Activity Description is required", cut.Markup);
    }

    [Fact]
    public async Task Validate_Shows_DateBusinessStarted_Error_When_Missing_And_HavePaidEmployees_True()
    {
        WithPaidEmployees();

        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                PrincipalBusinessActivity = PrincipalBusinessActivityType.Retail,
                PrimaryBusinessActivityDescription = "Retail"
            });
        });

        await cut.InvokeAsync(cut.Instance.Validate);

        Assert.Contains("Date business started", cut.Markup);
    }

    [Fact]
    public async Task Validate_No_Date_Errors_When_HavePaidEmployees_Is_False()
    {
        // Date fields are not shown and not validated when HavePaidEmployees = false
        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, MakeValidModelNoDates());
        });

        await cut.InvokeAsync(cut.Instance.Validate);

        Assert.DoesNotContain("Date business started", cut.Markup);
    }

    // =========================================================
    // DATE VALIDATION EDGE CASES
    // =========================================================

    [Fact]
    public async Task Validate_Returns_False_When_DateBusinessStarted_Is_Future()
    {
        WithPaidEmployees();

        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                DateBusinessStarted = DateTime.Today.AddDays(1),
                DateFirstPaidEmployeesInWI = DateTime.Today.AddYears(-1),
                DateFirstPaidWagesInWI = DateTime.Today.AddYears(-1),
                PrincipalBusinessActivity = PrincipalBusinessActivityType.Retail,
                PrimaryBusinessActivityDescription = "Retail"
            });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.False(result);
    }

    [Fact]
    public async Task Validate_Returns_False_When_DateFirstPaidWages_Before_DateBusinessStarted()
    {
        WithPaidEmployees();

        var cut = Render<BusinessActivity>(p =>
        {
            p.Add(x => x.Model, new BusinessActivityModel
            {
                DateBusinessStarted = DateTime.Today.AddYears(-1),
                DateFirstPaidEmployeesInWI = DateTime.Today.AddMonths(-6),
                DateFirstPaidWagesInWI = DateTime.Today.AddYears(-2), // before business started
                PrincipalBusinessActivity = PrincipalBusinessActivityType.Retail,
                PrimaryBusinessActivityDescription = "Retail"
            });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.False(result);
    }
}
