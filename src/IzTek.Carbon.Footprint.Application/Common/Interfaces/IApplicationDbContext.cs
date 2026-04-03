namespace IzTek.Carbon.Footprint.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Role> Roles { get; }
    DbSet<User> Users { get; }
    DbSet<UserActivityAnswer> UserActivityAnswers { get; }
    DbSet<Goal> Goals { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<ActivityQuestion> ActivityQuestions { get; }
    DbSet<ActivityOption> ActivityOptions { get; }
    DbSet<UserActivityLog> UserActivityLogs { get; }
    DbSet<UsefulInformation> UsefulInformations { get; }
    DbSet<PollOption> PollOptions { get; }
    DbSet<PollSet> PollSets { get; }
    DbSet<PollQuestion> PollQuestions { get; }
    DbSet<TreeDefinition> TreeDefinitions { get; }
    DbSet<UserPollResult> UserPollResults { get; }
    DbSet<UserPollAnswer> UserPollAnswers { get; }
    DbSet<ScoringSetting> ScoringSettings { get; }
    DbSet<TreeDonation> TreeDonations { get; }
    DatabaseFacade Database { get; }
    DbSet<AppAsset> AppAssets { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}