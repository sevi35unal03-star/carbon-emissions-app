namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyQuestions
{
    public record DailyOptionResponse(
        Guid Id,
        string Text,
        double CarbonValue,
        Guid? NextQuestionId,
        DailyQuestionResponse? NextQuestion  // ✅ eklendi
    );

    public record DailyQuestionResponse(
        Guid Id,
        string Text,
        int DisplayOrder,
        List<DailyOptionResponse> Options,
        long RemainingSeconds
    );
}