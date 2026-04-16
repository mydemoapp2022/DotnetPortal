using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using UI.EmployerPortal.Generated.ServiceClients.EmployerRegistrationService;
using UI.EmployerPortal.Web.Features.EmployerRegistration;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Components;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Components.OwnershipForms;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Models;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Services;
using UI.EmployerPortal.Web.Features.Shared.FileUpload.Services;

namespace Test.UI.EmployerPortal.Web.Component.Features.EmployerRegistration;

public class OwnershipTests : BunitContext
{
    public OwnershipTests()
    {
        // EmployerRegistrationModelStore is internal — requires:
        // [assembly: InternalsVisibleTo("Test.UI.EmployerPortal.Web")] in the web project.
        // IUserAccountService is internal — requires:
        // [assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] for FakeItEasy proxy generation.
        Services.AddSingleton(A.Fake<IEmployerRegistrationService>());
        Services.AddSingleton(A.Fake<IUserAccountService>());
        Services.AddSingleton(A.Fake<IUploadServices>());
        Services.AddSingleton<EmployerRegistrationModelStore>();
    }

    // =========================================================
    // HELPER METHODS
    // =========================================================

    /// <summary>
    /// Valid QSF session data that passes all QSF section validation.
    /// Uses the "No" branch with a reason and defers the file upload.
    /// </summary>
    private static OwnershipSessionData MakeValidQsfSessionData()
    {
        return new OwnershipSessionData
        {
            OwnershipType = OwnershipType.QSF,
            QualifiedSettlementFund = new QualifiedSettlementFundModel
            {
                PaymentsForServices = false,
                PaymentReason = "Payments were not for services performed in Wisconsin",
                WillProvideDocumentationLater = true
            }
        };
    }

    /// <summary>
    /// Valid LLC documentation session data that passes all LLC doc section validation.
    /// HasRequiredDocumentation = true requires no further fields in ValidateSection.
    /// </summary>
    private static OwnershipSessionData MakeValidLlcDocSessionData()
    {
        return new OwnershipSessionData
        {
            OwnershipType = OwnershipType.LLCCorporation,
            LlcDocumentation = new LlcDocumentationModel
            {
                HasRequiredDocumentation = true
            }
        };
    }

    // =========================================================
    // RENDERING TESTS
    // =========================================================

    [Fact]
    public void Renders_Page_Title()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData());
        });

        Assert.Equal("Ownership", cut.Find("h1.ownership-title").TextContent.Trim());
    }

    [Fact]
    public void Renders_Ownership_Type_Label()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData());
        });

        Assert.Contains("Ownership Type", cut.Markup);
    }

    [Fact]
    public void Error_Banner_Not_Visible_By_Default()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData());
        });

        Assert.DoesNotContain("Ownership Type is required.", cut.Markup);
    }

    // =========================================================
    // CHILD FORM — DYNAMIC COMPONENT RENDERING
    // =========================================================

    [Fact]
    public void No_Child_Form_Rendered_When_OwnershipType_Is_None()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData());
        });

        Assert.Empty(cut.FindAll("div.corporation-section"));
        Assert.Empty(cut.FindAll("div.member-based-section"));
        Assert.Empty(cut.FindAll("div.sole-proprietorship-section"));
        Assert.Empty(cut.FindAll("div.limited-partnership-section"));
        Assert.Empty(cut.FindAll("div.estate-section"));
    }

    [Fact]
    public void Corporation_Form_Rendered_For_Corporation_Type()
    {
        var store = Services.GetRequiredService<EmployerRegistrationModelStore>();
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.HavePaidEmployeesForWorkInWisconsin = true;

        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Corporation });
        });

        Assert.NotEmpty(cut.FindAll("div.corporation-section"));
    }

    [Fact]
    public void Member_Based_Form_Rendered_For_LLC_Type()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.LLC });
        });

        Assert.NotEmpty(cut.FindAll("div.member-based-section"));
    }

    [Fact]
    public void Member_Based_Form_Rendered_For_LLCCorporation_Type()
    {
        // Suppress corporate officer services section so only the member-based form renders
        var store = Services.GetRequiredService<EmployerRegistrationModelStore>();
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.HavePaidEmployeesForWorkInWisconsin = true;

        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.LLCCorporation });
        });

        Assert.NotEmpty(cut.FindAll("div.member-based-section"));
    }

    [Fact]
    public void Member_Based_Form_Rendered_For_LLP_Type()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.LLP });
        });

        Assert.NotEmpty(cut.FindAll("div.member-based-section"));
    }

    [Fact]
    public void Member_Based_Form_Rendered_For_Partnership_Type()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Partnership });
        });

        Assert.NotEmpty(cut.FindAll("div.member-based-section"));
    }

    [Fact]
    public void Limited_Partnership_Form_Rendered_For_LP_Type()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.LP });
        });

        Assert.NotEmpty(cut.FindAll("div.limited-partnership-section"));
    }

    [Fact]
    public void Sole_Proprietorship_Form_Rendered_For_SoleProprietorship_Type()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.SoleProprietorship });
        });

        Assert.NotEmpty(cut.FindAll("div.sole-proprietorship-section"));
    }

    [Fact]
    public void Sole_Proprietorship_Form_Rendered_For_Individual_Type()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Individual });
        });

        Assert.NotEmpty(cut.FindAll("div.sole-proprietorship-section"));
    }

    [Fact]
    public void Estate_Form_Rendered_For_Estate_Type()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Estate });
        });

        Assert.NotEmpty(cut.FindAll("div.estate-section"));
    }

    [Fact]
    public void No_Child_Form_Rendered_For_Association_Type()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Association });
        });

        Assert.Empty(cut.FindAll("div.corporation-section"));
        Assert.Empty(cut.FindAll("div.member-based-section"));
    }

    [Fact]
    public void No_Child_Form_Rendered_For_Cooperative_Type()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Cooperative });
        });

        Assert.Empty(cut.FindAll("div.corporation-section"));
        Assert.Empty(cut.FindAll("div.member-based-section"));
    }

    [Fact]
    public void No_Child_Form_Rendered_For_Union_Type()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Union });
        });

        Assert.Empty(cut.FindAll("div.corporation-section"));
        Assert.Empty(cut.FindAll("div.member-based-section"));
    }

    // =========================================================
    // CORPORATE OFFICER SERVICES SECTION
    // =========================================================

    [Fact]
    public void Corporate_Officer_Services_Section_Shown_For_Corporation_With_No_Payroll()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Corporation });
        });

        Assert.NotEmpty(cut.FindAll("div.corporate-officer-services-section"));
    }

    [Fact]
    public void Corporate_Officer_Services_Section_Shown_For_LLCCorporation_With_No_Payroll()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.LLCCorporation });
        });

        Assert.NotEmpty(cut.FindAll("div.corporate-officer-services-section"));
    }

    [Fact]
    public void Corporate_Officer_Services_Section_Not_Shown_When_HasPaidEmployees_Is_True()
    {
        var store = Services.GetRequiredService<EmployerRegistrationModelStore>();
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.HavePaidEmployeesForWorkInWisconsin = true;

        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Corporation });
        });

        Assert.Empty(cut.FindAll("div.corporate-officer-services-section"));
    }

    [Fact]
    public void Corporate_Officer_Services_Section_Not_Shown_When_ExpectsFuturePayroll_Is_True()
    {
        var store = Services.GetRequiredService<EmployerRegistrationModelStore>();
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.ExpectFuturePayroll = true;

        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Corporation });
        });

        Assert.Empty(cut.FindAll("div.corporate-officer-services-section"));
    }

    [Fact]
    public void Corporate_Officer_Services_Section_Not_Shown_For_LLC_Type()
    {
        // Only Corporation and LLCCorporation qualify for this section
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.LLC });
        });

        Assert.Empty(cut.FindAll("div.corporate-officer-services-section"));
    }

    [Fact]
    public void Corporate_Officer_Services_Section_Not_Shown_When_OwnershipType_Is_None()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData());
        });

        Assert.Empty(cut.FindAll("div.corporate-officer-services-section"));
    }

    // =========================================================
    // LLC DOCUMENTATION SECTION
    // =========================================================

    [Fact]
    public void LLC_Documentation_Section_Shown_For_LLCCorporation_Type()
    {
        var store = Services.GetRequiredService<EmployerRegistrationModelStore>();
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.HavePaidEmployeesForWorkInWisconsin = true;

        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.LLCCorporation });
        });

        Assert.NotEmpty(cut.FindAll("div.llc-documentation-section"));
    }

    [Fact]
    public void LLC_Documentation_Section_Not_Shown_For_LLC_Type()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.LLC });
        });

        Assert.Empty(cut.FindAll("div.llc-documentation-section"));
    }

    [Fact]
    public void LLC_Documentation_Section_Not_Shown_For_Corporation_Type()
    {
        var store = Services.GetRequiredService<EmployerRegistrationModelStore>();
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.HavePaidEmployeesForWorkInWisconsin = true;

        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Corporation });
        });

        Assert.Empty(cut.FindAll("div.llc-documentation-section"));
    }

    [Fact]
    public void LLC_Documentation_Section_Not_Shown_When_OwnershipType_Is_None()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData());
        });

        Assert.Empty(cut.FindAll("div.llc-documentation-section"));
    }

    // =========================================================
    // QUALIFIED SETTLEMENT FUND (QSF) SECTION
    // =========================================================

    [Fact]
    public void QSF_Section_Shown_For_QSF_Type_With_No_Payroll_Flags()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.QSF });
        });

        Assert.NotEmpty(cut.FindAll("div.qsf-section"));
    }

    /// <summary>
    /// QSF section condition: (!HasPaidEmployeesInWI || !ExpectsFuturePayroll).
    /// The section is visible when at least one payroll flag is false.
    /// </summary>
    [Fact]
    public void QSF_Section_Shown_For_QSF_Type_When_Only_HasPaidEmployees_Is_True()
    {
        var store = Services.GetRequiredService<EmployerRegistrationModelStore>();
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.HavePaidEmployeesForWorkInWisconsin = true;

        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.QSF });
        });

        Assert.NotEmpty(cut.FindAll("div.qsf-section"));
    }

    [Fact]
    public void QSF_Section_Shown_For_QSF_Type_When_Only_ExpectsFuturePayroll_Is_True()
    {
        var store = Services.GetRequiredService<EmployerRegistrationModelStore>();
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.ExpectFuturePayroll = true;

        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.QSF });
        });

        Assert.NotEmpty(cut.FindAll("div.qsf-section"));
    }

    [Fact]
    public void QSF_Section_Not_Shown_For_QSF_Type_When_Both_Payroll_Flags_Are_True()
    {
        var store = Services.GetRequiredService<EmployerRegistrationModelStore>();
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.HavePaidEmployeesForWorkInWisconsin = true;
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.ExpectFuturePayroll = true;

        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.QSF });
        });

        Assert.Empty(cut.FindAll("div.qsf-section"));
    }

    [Fact]
    public void QSF_Section_Not_Shown_For_Non_QSF_Type()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.LLC });
        });

        Assert.Empty(cut.FindAll("div.qsf-section"));
    }

    [Fact]
    public void QSF_Section_Not_Shown_When_OwnershipType_Is_None()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData());
        });

        Assert.Empty(cut.FindAll("div.qsf-section"));
    }

    // =========================================================
    // CONTENT TESTS — UNIQUE TEXT IN CHILD FORMS/SECTIONS
    // =========================================================

    [Fact]
    public void Corporation_Form_Shows_Headquartered_Outside_USA_Checkbox()
    {
        var store = Services.GetRequiredService<EmployerRegistrationModelStore>();
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.HavePaidEmployeesForWorkInWisconsin = true;

        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Corporation });
        });

        Assert.Contains("The business is headquartered outside of the United States", cut.Markup);
    }

    [Fact]
    public void Member_Based_Form_Shows_Owner_Information_Header()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.LLC });
        });

        Assert.Contains("Owner Information", cut.Markup);
    }

    [Fact]
    public void LP_Form_Shows_General_Partner_Information_Header()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.LP });
        });

        Assert.Contains("General Partner Information", cut.Markup);
    }

    [Fact]
    public void Estate_Form_Shows_Decedent_Section()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Estate });
        });

        Assert.Contains("Decedent", cut.Markup);
    }

    [Fact]
    public void Corporate_Officer_Services_Section_Shows_Not_Paid_Employees_Message()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Corporation });
        });

        Assert.Contains("not paid employees and does not expect to pay employees", cut.Markup);
    }

    [Fact]
    public void LLC_Documentation_Section_Shows_Required_Documentation_Header()
    {
        var store = Services.GetRequiredService<EmployerRegistrationModelStore>();
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.HavePaidEmployeesForWorkInWisconsin = true;

        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.LLCCorporation });
        });

        Assert.Contains("Required Documentation", cut.Markup);
    }

    [Fact]
    public void QSF_Section_Shows_Payments_For_Services_Question()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.QSF });
        });

        Assert.Contains("Were the payments for services?", cut.Markup);
    }

    // =========================================================
    // VALIDATION TESTS
    // =========================================================

    [Fact]
    public async Task Validate_Returns_False_When_OwnershipType_Is_None()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData());
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.False(result);
    }

    [Fact]
    public async Task Validate_Shows_Ownership_Type_Required_Error_When_None_Selected()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData());
        });

        await cut.InvokeAsync(cut.Instance.Validate);

        Assert.Contains("Ownership Type is required.", cut.Markup);
    }

    [Fact]
    public async Task Validate_Shows_Error_Banner_When_OwnershipType_Is_None()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData());
        });

        await cut.InvokeAsync(cut.Instance.Validate);
        cut.WaitForState(() => cut.Markup.Contains("Ownership Type is required."));

        Assert.Contains("Ownership Type is required.", cut.Markup);
    }

    [Fact]
    public async Task Validate_Returns_True_For_Association_Type()
    {
        // No sub-form or extra sections → type set + no child errors = valid
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Association });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.True(result);
    }

    [Fact]
    public async Task Validate_Returns_True_For_Cooperative_Type()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Cooperative });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.True(result);
    }

    [Fact]
    public async Task Validate_Returns_True_For_Union_Type()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.Union });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.True(result);
    }

    [Fact]
    public async Task Validate_Returns_True_For_City_Government_Agency_Type()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.CityGovernmentAgency });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.True(result);
    }

    /// <summary>
    /// Both payroll flags true → ShowQsfSection = false → no child validation errors,
    /// so Validate() returns true for QSF type without needing a valid QSF sub-model.
    /// </summary>
    [Fact]
    public async Task Validate_Returns_True_For_QSF_Type_When_Both_Payroll_Flags_True()
    {
        var store = Services.GetRequiredService<EmployerRegistrationModelStore>();
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.HavePaidEmployeesForWorkInWisconsin = true;
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.ExpectFuturePayroll = true;

        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, new OwnershipSessionData { OwnershipType = OwnershipType.QSF });
        });

        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.True(result);
    }

    /// <summary>
    /// After a failed validation for None type, changing to a valid type and re-validating
    /// should clear the error message and return true.
    /// </summary>
    [Fact(Skip = "")]
    public async Task Validate_Ownership_Type_Error_Cleared_After_Valid_Type_Selected()
    {
        var model = new OwnershipSessionData { OwnershipType = OwnershipType.None };
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, model);
        });

        await cut.InvokeAsync(cut.Instance.Validate);
        cut.WaitForState(() => cut.Markup.Contains("Ownership Type is required."));

        model.OwnershipType = OwnershipType.Association;
        var result = await cut.InvokeAsync(cut.Instance.Validate);

        Assert.True(result);
        Assert.DoesNotContain("Ownership Type is required.", cut.Markup);
    }

    // =========================================================
    // STATE RESTORATION TESTS
    // =========================================================

    /// <summary>
    /// When a model with existing LLC documentation data is passed on init,
    /// the section renders and the child form receives the pre-populated model.
    /// </summary>
    [Fact]
    public void Restores_LlcDocumentation_From_Model_On_Init()
    {
        var store = Services.GetRequiredService<EmployerRegistrationModelStore>();
        store.EmployerRegistrationModel.PreliminaryQuestionsModel.HavePaidEmployeesForWorkInWisconsin = true;

        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, MakeValidLlcDocSessionData());
        });

        Assert.NotEmpty(cut.FindAll("div.llc-documentation-section"));
    }

    /// <summary>
    /// When a model with existing QSF data is passed on init,
    /// the section renders and the child form receives the pre-populated model.
    /// </summary>
    [Fact]
    public void Restores_QSF_Data_From_Model_On_Init()
    {
        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, MakeValidQsfSessionData());
        });

        Assert.NotEmpty(cut.FindAll("div.qsf-section"));
    }

    [Fact]
    public void Restores_CorporateOfficerServices_From_Model_On_Init()
    {
        var model = new OwnershipSessionData
        {
            OwnershipType = OwnershipType.Corporation,
            CorporateOfficerServices = new CorporateOfficerServicesModel()
        };

        var cut = Render<Ownership>(p =>
        {
            p.Add(x => x.Model, model);
        });

        Assert.NotEmpty(cut.FindAll("div.corporate-officer-services-section"));
    }
}
