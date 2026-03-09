using Iztek.Carbon.Footprint.Domain.Entities;

namespace IzTek.Carbon.Footprint.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Role> Roles { get; set; }

    DbSet<User> Users { get; set; }
    DbSet<UserAnswer> UserAnswers { get; set; }


    DbSet<Product> Products { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<ActivityQuestion> ActivityQuestions { get; set; }
    DbSet<ActivityOption> ActivityOptions { get; set; }
    DbSet<UserActivityLog> UserActivityLogs { get; set; }
    DbSet<UsefulInformation> UsefulInformations { get; set; }
    DbSet<PollOption> PollOptions { get; set; }
    DbSet<PollSet> PollSets { get; set; }
    DbSet<PollQuestion> PollQuestions { get; set; }
    DbSet<TreeDefinition> TreeDefinitions { get; set; }
    DbSet<UserPollResult> UserPollResults { get; set; }

    DbSet<ScoringSetting> ScoringSettings { get; set; }

    DatabaseFacade Database { get; }
  

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
