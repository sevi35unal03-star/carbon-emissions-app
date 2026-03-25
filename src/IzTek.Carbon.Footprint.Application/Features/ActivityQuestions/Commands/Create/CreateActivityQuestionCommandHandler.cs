using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.SendPush;

namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Create;

public static class CreateActivityQuestionCommandHandler
{
    public static async Task<Result> HandleAsync(
        CreateActivityQuestionCommand command,
        IApplicationDbContext context,
        IMessageBus bus,
        CancellationToken ct)
    {
        // Aynı gün için maksimum 2 soru kontrolü
        var existingCount = await context.ActivityQuestions
            .CountAsync(x => x.IsActive
                          && x.StartDate.Date == command.StartDate.Date, ct);

        if (existingCount >= 2)
            return Result.Failure(
                SystemErrorCodes.MaxDailyQuestionLimitReached, HttpStatusCode.BadRequest);

        var question = new ActivityQuestion(
            command.Text,
            command.ScheduledTime,
            command.DisplayOrder,
            command.StartDate,
            command.EndDate);

        foreach (var opt in command.Options)
            question.AddOption(opt.Text, opt.CarbonValue, opt.NextQuestionId);

        await context.ActivityQuestions.AddAsync(question, ct);

        // Zamanlanmış bildirim
        var notificationDate = command.StartDate.Date.Add(command.ScheduledTime);
        if (notificationDate > DateTime.UtcNow)
        {
            await bus.ScheduleAsync(
                new SendQuestionPushNotificationCommand(question.Id),
                notificationDate);
        }

        await context.SaveChangesAsync(ct);

        return Result.Created();
    }
}