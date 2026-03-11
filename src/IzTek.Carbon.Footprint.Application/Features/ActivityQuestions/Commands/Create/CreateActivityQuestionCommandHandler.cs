
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
        var question = new ActivityQuestion(
            command.Text,
            command.ScheduledTime,
            command.DisplayOrder,
            command.StartDate,
            command.EndDate);

        foreach (var opt in command.Options)
            question.AddOption(opt.Text, opt.CarbonValue, opt.NextQuestionId);

        await context.ActivityQuestions.AddAsync(question, ct);
       

        // ✅ Zamanlanmış bildirim
        var notificationDate = command.StartDate.Date.Add(command.ScheduledTime);
        if (notificationDate > DateTime.UtcNow)
        {
            await bus.ScheduleAsync(
                new SendQuestionPushNotificationCommand(question.Id), // ✅ PollQuestionId → Id
                notificationDate);                                     // ✅ DeliveryOptions yerine ScheduleAsync
        }

        await context.SaveChangesAsync(ct);

        return Result.Created();


    } 
}