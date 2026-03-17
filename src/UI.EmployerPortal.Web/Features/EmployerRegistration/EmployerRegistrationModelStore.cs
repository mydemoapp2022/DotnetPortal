using UI.EmployerPortal.Generated.ServiceClients.EmployerRegistrationService;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Models;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Services;
using PortalQuestionResponseItem = UI.EmployerPortal.Generated.ServiceClients.EmployerRegistrationService.PortalQuestionResponseItem;
using SurveyResponses = UI.EmployerPortal.Generated.ServiceClients.EmployerRegistrationService.SurveyResponses;

namespace UI.EmployerPortal.Web.Features.EmployerRegistration;

internal class EmployerRegistrationModelStore
{
    private readonly IEmployerRegistrationService _employerRegistrationService;
    private readonly IUserAccountService _userAccountService;

    public EmployerRegistrationModel EmployerRegistrationModel { get; set; } = new();

    public EmployerRegistrationModelStore(
        IEmployerRegistrationService employerRegistrationService,
        IUserAccountService userAccountService)
    {
        _employerRegistrationService = employerRegistrationService;
        _userAccountService = userAccountService;
    }

    public async void Save()
    {
        var registrationQuestions = await _employerRegistrationService.LoadAllEmployerRegistrationQuestionsAsync();
        var surveyResponses = new List<PortalQuestionResponseItem>();

        foreach (var modelResponse in EmployerRegistrationModel.GetSurveyResponses())
        {
            var match = registrationQuestions.Questions.FirstOrDefault(q =>
            {
                return q.QuestionSetItemSK == modelResponse._surveyResponseItemSk;
            });

            if (match != null)
            {
                surveyResponses.Add(new PortalQuestionResponseItem()
                {
                    QuestionSetItemSK = match.QuestionSetItemSK,
                    QuestionSetCodeSK = match.QuestionSetCodeSK,
                    ReplyEntryTime = DateTime.Now,
                    ReplyText = modelResponse._response,
                });
            }
        }

        var secureUserSkClaim = _userAccountService.GetUserSKClaim();

        var saveRequest = new SurveyResponses()
        {
            SecureUserSK = secureUserSkClaim,
            SurveyStartDate = DateTime.Now,
            SurveyCompletionDate = DateTime.Now,
            SurveyNumberText = string.Empty, // NOTE: this will probably need to change as we add the ability to load a partially completed survey
            Responses = surveyResponses.ToArray(),
        };

        var response = await _employerRegistrationService.SavePortalResponsesAsync(saveRequest);
    }
}
