using System.ComponentModel.DataAnnotations;
using UI.EmployerPortal.Generated.ServiceClients.EmployerRegistrationService;
using UI.EmployerPortal.Razor.SharedComponents.Model;

namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// 
/// </summary>
public class PreliminaryQuestionsModel : IEmployerRegistrationModelSection
{
    /// <summary>
    /// Federal Employer Identification Number.
    /// Validation is handled entirely by <see cref="UI.EmployerPortal.Razor.SharedComponents.Inputs.FEINField.ValidateFEIN"/>.
    /// </summary>
    public string? FEIN { get; set; }

    /// <summary>
    /// Unemployment Isurance Employer Account Number
    /// </summary>
    public string UIAccountNumber { get; set; } = string.Empty;


    //public BusinessCategory? BusinessCategory { get; set; } = null;

    /// <summary>
    /// Answer to "Are you a non-profit organization as described in s.501(c)(3) of the IRS code?"
    /// Drives the entire 501(c)(3) sub-tree visibility.
    /// </summary>
    public bool? IsNonProfitOrg { get; set; } = null;

    /// <summary>
    /// Answer to "Do you have a 501(c)(3) ruling from the IRS?"
    /// Shown when IsNonProfit501c3 is true.
    /// </summary>
    public bool? HasRulingFrom501c3IRS { get; set; } = null;

    /// <summary>
    /// Answer to "Have you applied for 501(c)(3) status with the IRS?"
    /// Shown when IsNonProfit501c3 is true and HasRulingFrom501c3IRS is false.
    /// </summary>
    public bool? HasAppliedFor501c3WithIRS { get; set; } = null;

    /// <summary>
    /// Checkbox: "I will supply required documentation at a later date."
    /// Shown on all 501(c)(3) document upload paths as an alternative to uploading immediately.
    /// </summary>
    public bool WillSupplyDocumentationLater { get; set; } = false;

    /// <summary>
    ///
    /// </summary>
    public bool? AcquiredExistingBusiness { get; set; } = null;

    /// <summary>
    /// 
    /// </summary>
    public bool? KnowAcquiredBusinessAccountNumber { get; set; } = null;

    /// <summary>
    /// 
    /// </summary>
    public string AcquiredBusinessAccountNumber { get; set; } = string.Empty;

    /// <summary>
    ///
    /// </summary>
    [MaxLength(128, ErrorMessage = "Acquired Business Name cannot exceed 128 characters")]
    public string AcquiredBusinessName { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public AddressModel? AcquiredBusinessAddress { get; set; } = null;

    /// <summary>
    /// 
    /// </summary>
    public bool? HavePaidEmployeesForWorkInWisconsin { get; set; } = null;

    /// <summary>
    /// 
    /// </summary>
    public bool? HaveEmployeesCurrentlyWorkingInWisconsin { get; set; } = null;

    /// <summary>
    /// 
    /// </summary>
    public DateOnly? LastEmploymentDate { get; set; } = null;

    /// <summary>
    /// 
    /// </summary>
    public DateOnly? LastPayrollDate { get; set; } = null;

    /// <summary>
    /// 
    /// </summary>
    public bool? ExpectFuturePayroll { get; set; } = null;

    /// <summary>
    /// Acknowledgement checkbox shown when the employer confirms they still have paid employees in Wisconsin.
    /// </summary>
    public bool InformationIsAccurate { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    public FuturePayPeriod? ExpectedFuturePayrollPeriod { get; set; } = null;

    ///// <summary>
    ///// 
    ///// </summary>
    //public bool? HaveSoldOrTransferredBusiness { get; set; } = null;

    /// <summary>
    /// 
    /// </summary>
    public string? NoEmployeeExplanation { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? PEOName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? PEOUIAccountNumber { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? PEOFEIN { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateOnly? LeasingStartDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? FiscalAgentName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? FiscalAgentUIAccountNumber { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? OtherReason { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public NoEmployeeReason? SelectedNoEmployeeReason { get; set; } = null;


    /// <inheritdoc/>
    public List<Tuple<RegistrationAddressCode, AddressModel>> GetSurveyAddresses()
    {
        var addresses = new List<Tuple<RegistrationAddressCode, AddressModel>>();

        if (AcquiredBusinessAddress != null)
        {
            addresses.Add(Tuple.Create(RegistrationAddressCode.Acquired_Business, AcquiredBusinessAddress));
        }

        return addresses;
    }

    /// <inheritdoc/>
    public void LoadSurveyAddresses(RegistrationAddressProxy[] addresses)
    {
        if (IEmployerRegistrationModelSection.FindAddressHelper(addresses, RegistrationAddressCode.Acquired_Business, out var acquiredBusinessAddress))
        {
            AcquiredBusinessAddress = IEmployerRegistrationModelSection.ConvertAddressResponseToModel(acquiredBusinessAddress);
        }
    }

    /// <inheritdoc/>
    public List<SurveyContact> GetSurveyContacts()
    {
        return new();
    }

    /// <inheritdoc/>
    public void LoadSurveyContacts(RegistrationIndividualProxy[] contacts)
    {
        return;
    }

    /// <inheritdoc />
    public List<SurveyResponse> GetSurveyResponses()
    {
        var responses = new List<SurveyResponse>();

        // Visibility helpers
        var show501c3SubTree = IsNonProfitOrg == true;
        var visibilityQ2_1 = AcquiredExistingBusiness == true;
        var visibilityQ2_1_1 = visibilityQ2_1 && KnowAcquiredBusinessAccountNumber == true;
        var visibilityQ2_1_2 = visibilityQ2_1 && KnowAcquiredBusinessAccountNumber == false;
        var visibilityQ3 = AcquiredExistingBusiness == false;
        var visibilityQ3_1 = visibilityQ3 && HavePaidEmployeesForWorkInWisconsin == true;
        var visibilityQ3_2 = visibilityQ3 && HavePaidEmployeesForWorkInWisconsin == false;
        var visibilityQ3_2_1 = visibilityQ3_2 && ExpectFuturePayroll == true;
        var visibilityQ4 = visibilityQ3_1 && HaveEmployeesCurrentlyWorkingInWisconsin == false;

        if (!string.IsNullOrWhiteSpace(FEIN))
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.FEIN_NUM, _response = FEIN.Replace("-", string.Empty), _responseDisplay = FEIN });
        }

        if (!string.IsNullOrWhiteSpace(UIAccountNumber))
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.ER_ACCT_NUM, _response = UIAccountNumber.Replace("-", string.Empty), _responseDisplay = UIAccountNumber });
        }

        if (IsNonProfitOrg.HasValue)
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_NON_PRFT_FLG, _response = IEmployerRegistrationModelSection.ConvertBooleanResponseToString(IsNonProfitOrg.Value), _responseDisplay = IEmployerRegistrationModelSection.ConvertBooleanResponseToDisplayString(IsNonProfitOrg.Value) });
        }

        if (show501c3SubTree && HasRulingFrom501c3IRS.HasValue)
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_501C3_RULING_FLG, _response = IEmployerRegistrationModelSection.ConvertBooleanResponseToString(HasRulingFrom501c3IRS.Value), _responseDisplay = IEmployerRegistrationModelSection.ConvertBooleanResponseToDisplayString(HasRulingFrom501c3IRS.Value) });

            if (HasRulingFrom501c3IRS == true && WillSupplyDocumentationLater)
            {
                responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_MISS_DOC_501C3_RULING, _response = "Yes" });
            }
        }

        if (show501c3SubTree && HasRulingFrom501c3IRS == false && HasAppliedFor501c3WithIRS.HasValue)
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_APPLY_501C3, _response = IEmployerRegistrationModelSection.ConvertBooleanResponseToString(HasAppliedFor501c3WithIRS.Value), _responseDisplay = IEmployerRegistrationModelSection.ConvertBooleanResponseToDisplayString(HasAppliedFor501c3WithIRS.Value) });

            if (HasAppliedFor501c3WithIRS == true && WillSupplyDocumentationLater)
            {
                responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_MISS_DOC_ARTCL_INCORP, _response = "Yes" });
                responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_MISS_DOC_IRS_APP_ACCPT, _response = "Yes" });
            }
        }

        if (AcquiredExistingBusiness.HasValue)
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.ACQ_BUS_FLG, _response = IEmployerRegistrationModelSection.ConvertBooleanResponseToString(AcquiredExistingBusiness.Value), _responseDisplay = IEmployerRegistrationModelSection.ConvertBooleanResponseToDisplayString(AcquiredExistingBusiness.Value) });
        }

        if (visibilityQ2_1 && KnowAcquiredBusinessAccountNumber.HasValue)
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.ACQ_ACCT_NUM_KNWN, _response = IEmployerRegistrationModelSection.ConvertBooleanResponseToString(KnowAcquiredBusinessAccountNumber.Value), _responseDisplay = IEmployerRegistrationModelSection.ConvertBooleanResponseToDisplayString(KnowAcquiredBusinessAccountNumber.Value) });
        }

        if (visibilityQ2_1_1 && !string.IsNullOrWhiteSpace(AcquiredBusinessAccountNumber))
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.ACQ_BUS_ACCT_NUM, _response = AcquiredBusinessAccountNumber.Replace("-", string.Empty), _responseDisplay = AcquiredBusinessAccountNumber });
        }

        if (visibilityQ2_1_2 && !string.IsNullOrWhiteSpace(AcquiredBusinessName))
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.ACQ_BUS_NAM, _response = AcquiredBusinessName });
        }

        if (visibilityQ3 && HavePaidEmployeesForWorkInWisconsin.HasValue)
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PAID_EE_FLG, _response = IEmployerRegistrationModelSection.ConvertBooleanResponseToString(HavePaidEmployeesForWorkInWisconsin.Value), _responseDisplay = IEmployerRegistrationModelSection.ConvertBooleanResponseToDisplayString(HavePaidEmployeesForWorkInWisconsin.Value) });
        }

        if (visibilityQ3_1 && HaveEmployeesCurrentlyWorkingInWisconsin.HasValue)
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.STILL_EE_FLG, _response = IEmployerRegistrationModelSection.ConvertBooleanResponseToString(HaveEmployeesCurrentlyWorkingInWisconsin.Value), _responseDisplay = IEmployerRegistrationModelSection.ConvertBooleanResponseToDisplayString(HaveEmployeesCurrentlyWorkingInWisconsin.Value) });
        }

        if (visibilityQ3_2 && ExpectFuturePayroll.HasValue)
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.EXPT_PAY_EE_FLG, _response = IEmployerRegistrationModelSection.ConvertBooleanResponseToString(ExpectFuturePayroll.Value), _responseDisplay = IEmployerRegistrationModelSection.ConvertBooleanResponseToDisplayString(ExpectFuturePayroll.Value) });
        }

        if (visibilityQ3_2_1 && ExpectedFuturePayrollPeriod.HasValue)
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.EXPT_PAY_EE_TIME, _response = ExpectedFuturePayrollPeriod.Value.ToString() });
        }

        if (visibilityQ4 && LastEmploymentDate.HasValue)
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.LAST_EMPL_DT, _response = LastEmploymentDate.Value.ToString("MM/dd/yyyy") });
        }

        if (visibilityQ4 && LastPayrollDate.HasValue)
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.LAST_PYRL_DT, _response = LastPayrollDate.Value.ToString("MM/dd/yyyy") });
        }

        if (visibilityQ4 && !string.IsNullOrWhiteSpace(PEOName))
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_PEO_NAME, _response = PEOName });
        }

        if (visibilityQ4 && !string.IsNullOrWhiteSpace(PEOUIAccountNumber))
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_PEO_NUM, _response = PEOUIAccountNumber.Replace("-", string.Empty) });
        }

        if (visibilityQ4 && !string.IsNullOrWhiteSpace(PEOFEIN))
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_PEO_NUM, _response = PEOFEIN.Replace("-", string.Empty) });
        }

        if (visibilityQ4 && LeasingStartDate.HasValue)
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_PEO_DATE, _response = LeasingStartDate.Value.ToString("MM/dd/yyyy") });
        }

        if (visibilityQ4 && !string.IsNullOrWhiteSpace(NoEmployeeExplanation))
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_BUS_WITHOUT_EE, _response = NoEmployeeExplanation });
        }

        if (visibilityQ4 && !string.IsNullOrWhiteSpace(OtherReason))
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_OTHR_RSN, _response = OtherReason });
        }

        if (visibilityQ4 && !string.IsNullOrWhiteSpace(FiscalAgentName))
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_FSCL_AGNT_NAM, _response = FiscalAgentName });
        }

        if (visibilityQ4 && !string.IsNullOrWhiteSpace(FiscalAgentUIAccountNumber))
        {
            responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_FA_UI_ACCT_NUM, _response = FiscalAgentUIAccountNumber.Replace("-", string.Empty) });
        }

        return responses;
    }

    /// <inheritdoc/>
    public void LoadSurveyResponses(SurveyResponseItemProxy[] responses)
    {
        //if (!string.IsNullOrWhiteSpace(FEIN))
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.FEIN_NUM, _response = FEIN.Replace("-", string.Empty) });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.FEIN_NUM, out var fein))
        {
            FEIN = IEmployerRegistrationModelSection.FormatFeinResponseString(fein.ReplyText);
        }

        //if (!string.IsNullOrWhiteSpace(UIAccountNumber))
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.ER_ACCT_NUM, _response = UIAccountNumber.Replace("-", string.Empty) });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.ER_ACCT_NUM, out var employerUiAccountNumber))
        {
            UIAccountNumber = IEmployerRegistrationModelSection.FormatUiAccountNumberResponseString(employerUiAccountNumber.ReplyText);
        }

        //if (Is501c3.HasValue)
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_NON_PRFT_FLG, _response = IEmployerRegistrationModelSection.ConvertBooleanResponseToString(Is501c3.Value), _responseDisplay = IEmployerRegistrationModelSection.ConvertBooleanResponseToDisplayString(Is501c3.Value) });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.PRTL_NON_PRFT_FLG, out var is501c3))
        {
            IsNonProfitOrg = IEmployerRegistrationModelSection.ConvertResponseStringToBoolean(is501c3.ReplyText);
        }

        //if (AcquiredExistingBusiness.HasValue)
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.ACQ_BUS_FLG, _response = IEmployerRegistrationModelSection.ConvertBooleanResponseToString(AcquiredExistingBusiness.Value) });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.ACQ_BUS_FLG, out var acquiredExistingBusiness))
        {
            AcquiredExistingBusiness = IEmployerRegistrationModelSection.ConvertResponseStringToBoolean(acquiredExistingBusiness.ReplyText);
        }

        //if (KnowAcquiredBusinessAccountNumber.HasValue)
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.ACQ_ACCT_NUM_KNWN, _response = IEmployerRegistrationModelSection.ConvertBooleanResponseToString(KnowAcquiredBusinessAccountNumber.Value) });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.ACQ_ACCT_NUM_KNWN, out var knowAcquiredBusinessAccountNumber))
        {
            KnowAcquiredBusinessAccountNumber = IEmployerRegistrationModelSection.ConvertResponseStringToBoolean(knowAcquiredBusinessAccountNumber.ReplyText);
        }

        //if (!string.IsNullOrWhiteSpace(AcquiredBusinessAccountNumber))
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.ACQ_BUS_ACCT_NUM, _response = AcquiredBusinessAccountNumber });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.ACQ_BUS_ACCT_NUM, out var acquiredBusinessUiAccountNumber))
        {
            AcquiredBusinessAccountNumber = IEmployerRegistrationModelSection.FormatUiAccountNumberResponseString(acquiredBusinessUiAccountNumber.ReplyText);
        }

        //if (!string.IsNullOrWhiteSpace(AcquiredBusinessName))
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.ACQ_BUS_NAM, _response = AcquiredBusinessName });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.ACQ_BUS_NAM, out var acquiredBusinessName))
        {
            AcquiredBusinessName = acquiredBusinessName.ReplyText;
        }

        //if (HavePaidEmployeesForWorkInWisconsin.HasValue)
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PAID_EE_FLG, _response = IEmployerRegistrationModelSection.ConvertBooleanResponseToString(HavePaidEmployeesForWorkInWisconsin.Value) });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.PAID_EE_FLG, out var paidEeFlag))
        {
            HavePaidEmployeesForWorkInWisconsin = IEmployerRegistrationModelSection.ConvertResponseStringToBoolean(paidEeFlag.ReplyText);
        }

        //if (ExpectFuturePayroll.HasValue)
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.EXPT_PAY_EE_FLG, _response = IEmployerRegistrationModelSection.ConvertBooleanResponseToString(ExpectFuturePayroll.Value) });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.EXPT_PAY_EE_FLG, out var expectFuturePayroll))
        {
            ExpectFuturePayroll = IEmployerRegistrationModelSection.ConvertResponseStringToBoolean(expectFuturePayroll.ReplyText);
        }

        //if (ExpectedFuturePayrollPeriod.HasValue)
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.EXPT_PAY_EE_TIME, _response = ExpectedFuturePayrollPeriod.Value.ToString() });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.EXPT_PAY_EE_TIME, out var expectFuturePayrollPeriod)
            && Enum.TryParse<FuturePayPeriod>(expectFuturePayrollPeriod.ReplyText, out var expectFuturePayrollPeriodValue))
        {
            ExpectedFuturePayrollPeriod = expectFuturePayrollPeriodValue;
        }

        //if (HaveEmployeesCurrentlyWorkingInWisconsin.HasValue)
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.STILL_EE_FLG, _response = IEmployerRegistrationModelSection.ConvertBooleanResponseToString(HaveEmployeesCurrentlyWorkingInWisconsin.Value) });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.STILL_EE_FLG, out var haveEmployeesCurrentlyWorkingInWisconsin))
        {
            HaveEmployeesCurrentlyWorkingInWisconsin = IEmployerRegistrationModelSection.ConvertResponseStringToBoolean(haveEmployeesCurrentlyWorkingInWisconsin.ReplyText);
        }

        //if (LastEmploymentDate.HasValue)
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.LAST_EMPL_DT, _response = LastEmploymentDate.Value.ToString("MM/dd/yyyy") });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.LAST_EMPL_DT, out var lastEmploymentDate)
            && DateOnly.TryParse(lastEmploymentDate.ReplyText, out var lastEmploymentDateValue))
        {
            LastEmploymentDate = lastEmploymentDateValue;
        }

        //if (LastPayrollDate.HasValue)
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.LAST_PYRL_DT, _response = LastPayrollDate.Value.ToString("MM/dd/yyyy") });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.LAST_PYRL_DT, out var lastPayrollDate)
            && DateOnly.TryParse(lastPayrollDate.ReplyText, out var lastPayrollDateValue))
        {
            LastEmploymentDate = lastPayrollDateValue;
        }

        //if (!string.IsNullOrWhiteSpace(PEOName))
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_PEO_NAME, _response = PEOName });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.PRTL_PEO_NAME, out var peoName))
        {
            PEOName = peoName.ReplyText;
        }

        //if (!string.IsNullOrWhiteSpace(PEOUIAccountNumber))
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_PEO_NUM, _response = PEOUIAccountNumber });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.PRTL_PEO_NUM, out var peoUiAccountNumber))
        {
            PEOUIAccountNumber = IEmployerRegistrationModelSection.FormatUiAccountNumberResponseString(peoUiAccountNumber.ReplyText);
        }

        //if (!string.IsNullOrWhiteSpace(PEOFEIN))
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_PEO_NUM, _response = PEOFEIN });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.PRTL_PEO_NUM, out var peoFein))
        {
            PEOFEIN = IEmployerRegistrationModelSection.FormatFeinResponseString(peoFein.ReplyText);
        }

        //if (LeasingStartDate.HasValue)
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_PEO_DATE, _response = LeasingStartDate.Value.ToString("MM/dd/yyyy") });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.PRTL_PEO_DATE, out var peoDate)
            && DateOnly.TryParse(peoDate.ReplyText, out var peoDateValue))
        {
            LeasingStartDate = peoDateValue;
        }

        //if (!string.IsNullOrWhiteSpace(NoEmployeeExplanation))
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_BUS_WITHOUT_EE, _response = NoEmployeeExplanation });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.PRTL_BUS_WITHOUT_EE, out var noEeExplanation))
        {
            NoEmployeeExplanation = noEeExplanation.ReplyText;
        }

        //if (!string.IsNullOrWhiteSpace(OtherReason))
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_OTHR_RSN, _response = OtherReason });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.PRTL_OTHR_RSN, out var otherReason))
        {
            OtherReason = otherReason.ReplyText;
        }

        //if (!string.IsNullOrWhiteSpace(FiscalAgentName))
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_FSCL_AGNT_NAM, _response = FiscalAgentName });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.PRTL_FSCL_AGNT_NAM, out var fiscalAgentName))
        {
            FiscalAgentName = fiscalAgentName.ReplyText;
        }

        //if (!string.IsNullOrWhiteSpace(FiscalAgentUIAccountNumber))
        //{
        //    responses.Add(new SurveyResponse() { _surveyResponseItemSk = (int) SurveyResponseItem.PRTL_FA_UI_ACCT_NUM, _response = FiscalAgentUIAccountNumber });
        //}
        if (IEmployerRegistrationModelSection.FindResultHelper(responses, SurveyResponseItem.PRTL_FA_UI_ACCT_NUM, out var fiscalAgentUiAccountNumber))
        {
            FiscalAgentUIAccountNumber = IEmployerRegistrationModelSection.FormatUiAccountNumberResponseString(fiscalAgentUiAccountNumber.ReplyText);
        }
    }
}
