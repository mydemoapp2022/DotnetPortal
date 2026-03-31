using UI.EmployerPortal.Web.Features.EmployerRegistration.Models.UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// The model for the employer registration process
/// </summary>
public class EmployerRegistrationModel
{
    /// <summary>
    /// Step 1: The model for the preliminary questions part of the the emmployer registration process
    /// </summary>
    public PreliminaryQuestionsModel PreliminaryQuestionsModel { get; set; } = new();

    /// <summary>
    /// Step 2: The model for the ownership part of the employer registration process
    /// </summary>
    public OwnershipSessionData OwnershipSessionData { get; set; } = new();


    /// <summary>
    /// Step 3: The model for the Business Information as part of the the employer registration process
    /// </summary>
    public BusinessInformationModel BusinessInformationModel { get; set; } = new();

    /// <summary>
    /// Step 4: The model for the Business Contacts part of the the employer registration process
    /// </summary>
    public BusinessContactModel BusinessContactModel { get; set; } = new();

    /// <summary>
    /// Step 5: The model for the BusinessActivity part of the the employer registration process
    /// </summary>
    public BusinessActivityModel BusinessActivityModel { get; set; } = new();

    /// <summary>
    /// Get an aggregate list of question responses from the child models
    /// </summary>
    /// <returns></returns>
    public List<SurveyResponse> GetSurveyResponses()
    {
        var responses = new List<SurveyResponse>();
        responses.AddRange(GetPreliminaryQuestionsSurveyResponses());
        responses.AddRange(GetOwnershipQuestionsSurveyResponses());
        responses.AddRange(GetBusinessInformationSurveyResponses());
        responses.AddRange(GetBusinessContactSurveyResponses());
        responses.AddRange(GetBusinessActivitySurveyResponses());
        return responses;
    }
    /// <summary>
    /// PreliminaryQuestionsModel
    /// </summary>
    private List<SurveyResponse> GetPreliminaryQuestionsSurveyResponses()
    {
        return PreliminaryQuestionsModel.GetSurveyResponses();
    }
    /// <summary>
    /// OwnershipQuestionsModel
    /// </summary>
    /// <returns></returns>
    private List<SurveyResponse> GetOwnershipQuestionsSurveyResponses()
    {
        return OwnershipSessionData.GetSurveyResponses();
    }
    /// <summary>
    /// BusinessInformationModel
    /// </summary>
    private List<SurveyResponse> GetBusinessInformationSurveyResponses()
    {
        return BusinessInformationModel.GetSurveyResponses();
    }
    /// <summary>
    /// BusinessContactModel
    /// </summary>
    private List<SurveyResponse> GetBusinessContactSurveyResponses()
    {
        return BusinessContactModel.GetSurveyResponses();
    }
    /// <summary>
    /// BusinessActivityModel
    /// </summary>
    private List<SurveyResponse> GetBusinessActivitySurveyResponses()
    {
        return BusinessActivityModel.GetSurveyResponses();
    }
}
