using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IzTek.Carbon.Footprint.Persistence.Contexts;

public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>, IApplicationDbContext
{
    private readonly IMessageBus _bus;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IMessageBus bus) : base(options)
    {
        _bus = bus;
    }

    public DbSet<ActivityQuestion> ActivityQuestions => Set<ActivityQuestion>();
    public DbSet<ActivityOption> ActivityOptions => Set<ActivityOption>();
    public DbSet<UserActivityAnswer> UserActivityAnswers => Set<UserActivityAnswer>();
    public DbSet<UserActivityLog> UserActivityLogs => Set<UserActivityLog>();
    public DbSet<PollSet> PollSets => Set<PollSet>();
    public DbSet<PollQuestion> PollQuestions => Set<PollQuestion>();
    public DbSet<PollOption> PollOptions => Set<PollOption>();
    public DbSet<UserPollResult> UserPollResults => Set<UserPollResult>();
    public DbSet<UserPollAnswer> UserPollAnswers => Set<UserPollAnswer>();
    public DbSet<TreeDefinition> TreeDefinitions => Set<TreeDefinition>();
    public DbSet<TreeDonation> TreeDonations => Set<TreeDonation>();
    public DbSet<UsefulInformation> UsefulInformations => Set<UsefulInformation>();
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<ScoringSetting> ScoringSettings => Set<ScoringSetting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<AppAsset> AppAssets => Set<AppAsset>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                    ));
                }
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker.Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var entity in entities)
        {
            foreach (var @event in entity.DomainEvents)
                await _bus.PublishAsync(@event);

            entity.ClearDomainEvents();
        }

        return result;
    }
}