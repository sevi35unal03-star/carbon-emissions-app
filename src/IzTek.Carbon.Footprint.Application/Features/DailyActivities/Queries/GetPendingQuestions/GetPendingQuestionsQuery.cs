namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetPendingQuestions;
public record GetPendingQuestionsQuery();

public record PendingQuestionsResponse(
    bool HasPending,
    int PendingCount);