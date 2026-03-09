using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.SendPush;

public class SendQuestionPushNotificationHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IPlatformService _platformService;
    private readonly ILogger<SendQuestionPushNotificationHandler> _logger;

    public SendQuestionPushNotificationHandler(
        IApplicationDbContext context,
        IPlatformService platformService,
        ILogger<SendQuestionPushNotificationHandler> logger)
    {
        _context = context;
        _platformService = platformService;
        _logger = logger;
    }

    public async Task HandleAsync(
        SendQuestionPushNotificationCommand message,
        CancellationToken ct)
    {
        var question = await _context.ActivityQuestions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PollQuestionId == message.QuestionId, ct);

        if (question == null)
        {
            _logger.LogWarning("Notification failed: Question {Id} not found", message.QuestionId);
            return;
        }

        // ✅ IPushNotificationService yerine IPlatformService kullanıyoruz
        var result = await _platformService.SendPushToAllUsersAsync(
            "Günün Karbon Sorusu!",
            $"Bugünkü aktiviteni girmeyi unutma: {question.Text}",
            new { questionId = question.PollQuestionId });

        if (result is null || !result.IsSuccessful)
            _logger.LogWarning("Push notification gönderilemedi: Question {Id}", message.QuestionId);
        else
            _logger.LogInformation("Push notification gönderildi: Question {Id}", message.QuestionId);
    }
}