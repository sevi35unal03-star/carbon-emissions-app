namespace IzTek.Carbon.Footprint.Application.Features.RefreshTokens.Commands.Cleanup;

public static class CleanupExpiredRefreshTokensCommandHandler
{
    public static async Task Handle(
        CleanupExpiredRefreshTokensCommand command,
        IApplicationDbContext context,
        IMessageBus bus,
        CancellationToken ct)
    {
        var deleted = await context.RefreshTokens
            .Where(x => x.ExpiresAt < DateTime.UtcNow || x.IsRevoked)
            .ExecuteDeleteAsync(ct);

        // Kendini tekrar schedule et — her 24 saatte bir çalışır
        await bus.ScheduleAsync(
            new CleanupExpiredRefreshTokensCommand(),
            TimeSpan.FromHours(24));
    }
}