using System.Reflection;

namespace IzTek.Carbon.Footprint.Application.Common.Models;

public static class SystemErrorCodes
{
    private const string SERVICE_NAME = "System";

    public static readonly ErrorCode SystemError = new SystemErrorCode(1001, nameof(SystemError));
    public static readonly ErrorCode NotFound = new SystemErrorCode(1002, nameof(NotFound));
    public static readonly ErrorCode BadRequest = new SystemErrorCode(1003, nameof(BadRequest));
    public static readonly ErrorCode ValidationError = new SystemErrorCode(1004, nameof(ValidationError));
    public static readonly ErrorCode ConflictError = new SystemErrorCode(1005, nameof(ConflictError));
    public static readonly ErrorCode QuestionCreationFailed = new SystemErrorCode(1006, nameof(QuestionCreationFailed));

    public static readonly ErrorCode Unauthorized = new SystemErrorCode(1101, nameof(Unauthorized));
    public static readonly ErrorCode AccessDenied = new SystemErrorCode(1102, nameof(AccessDenied));
    public static readonly ErrorCode SessionExpired = new SystemErrorCode(1103, nameof(SessionExpired));

    public static readonly ErrorCode TooManyRequests = new SystemErrorCode(1201, nameof(TooManyRequests));

    public static readonly ErrorCode InvalidParameter = new SystemErrorCode(1301, nameof(InvalidParameter));

    public static readonly ErrorCode ProductAlreadyExists = new SystemErrorCode(1302, nameof(ProductAlreadyExists));

    public static readonly ErrorCode RoleAlreadyExists = new SystemErrorCode(1303, nameof(RoleAlreadyExists));
    public static readonly ErrorCode RoleNotFound = new SystemErrorCode(1304, nameof(RoleNotFound));

    // User hata kodları
    public static readonly ErrorCode UserNotFound = new SystemErrorCode(2001, nameof(UserNotFound));

    public static readonly ErrorCode NoPointsToDonat = new SystemErrorCode(2002, nameof(NoPointsToDonat));
    public static readonly ErrorCode InsufficientPoints = new SystemErrorCode(2003, nameof(InsufficientPoints)); // ← YENİ
    public static readonly ErrorCode TreeDefinitionNotFound = new SystemErrorCode(2004, nameof(TreeDefinitionNotFound)); // 2003 → 2004

    // UsefulInformation hata kodları
    public static readonly ErrorCode InformationAlreadyExists = new SystemErrorCode(3001, nameof(InformationAlreadyExists));

    public static readonly ErrorCode DeleteFailed = new SystemErrorCode(3002, nameof(DeleteFailed));
    public static readonly ErrorCode UpdateFailed = new SystemErrorCode(3005, nameof(UpdateFailed));

    // Goal hata kodları
    public static readonly ErrorCode GoalDataNotFound = new SystemErrorCode(4001, nameof(GoalDataNotFound));

    public static readonly ErrorCode GoalAlreadyExists = new SystemErrorCode(4002, nameof(GoalAlreadyExists));
    public static readonly ErrorCode GoalNotFound = new SystemErrorCode(4003, nameof(GoalNotFound));

    // Poll hata kodları
    public static readonly ErrorCode PollResultNotFound = new SystemErrorCode(5001, nameof(PollResultNotFound));

    public static readonly ErrorCode SourceQuestionIdsEmpty = new SystemErrorCode(5002, nameof(SourceQuestionIdsEmpty));
    public static readonly ErrorCode SourceQuestionsNotFound = new SystemErrorCode(5003, nameof(SourceQuestionsNotFound));
    public static readonly ErrorCode PollQuestionNotFound = new SystemErrorCode(5004, nameof(PollQuestionNotFound));
    public static readonly ErrorCode PollSetNotFound = new SystemErrorCode(5005, nameof(PollSetNotFound));
    public static readonly ErrorCode PollOptionNotFound = new SystemErrorCode(5006, nameof(PollOptionNotFound));
    public static readonly ErrorCode PollQuestionDeleteFailed = new SystemErrorCode(5007, nameof(PollQuestionDeleteFailed));
    public static readonly ErrorCode PollSetDeleteFailed = new SystemErrorCode(5008, nameof(PollSetDeleteFailed));
    public static readonly ErrorCode InvalidPollAnswers = new SystemErrorCode(5009, nameof(InvalidPollAnswers));
    public static readonly ErrorCode ActivePollNotFound = new SystemErrorCode(5010, nameof(ActivePollNotFound));
    public static readonly ErrorCode PollAlreadyAnswered = new SystemErrorCode(5011, nameof(PollAlreadyAnswered));

    // Scoring hata kodları
    public static readonly ErrorCode ScoringSettingsNotFound = new SystemErrorCode(6001, nameof(ScoringSettingsNotFound));

    public static readonly ErrorCode ScoringSettingsPartialNotFound = new SystemErrorCode(6002, nameof(ScoringSettingsPartialNotFound));

    // Activity hata kodları
    public static readonly ErrorCode InvalidActivityOption = new SystemErrorCode(7001, nameof(InvalidActivityOption));

    public static readonly ErrorCode ActivityNotFound = new SystemErrorCode(7002, nameof(ActivityNotFound));
    public static readonly ErrorCode PreviousAnswersNotFound = new SystemErrorCode(7003, nameof(PreviousAnswersNotFound));
    public static readonly ErrorCode ActivityQuestionNotFound = new SystemErrorCode(7004, nameof(ActivityQuestionNotFound));
    public static readonly ErrorCode MaxDailyQuestionLimitReached = new SystemErrorCode(7005, nameof(MaxDailyQuestionLimitReached)); // ← eklendi

    private sealed class SystemErrorCode(int code, string name) : ServiceErrorCode(code, name, SERVICE_NAME)
    {
    }

    public static IEnumerable<ErrorCode> GetAll()
    {
        return typeof(SystemErrorCodes)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(ErrorCode))
            .Select(f => (ErrorCode)f.GetValue(null)!)
            .Where(code => code != null);
    }
}