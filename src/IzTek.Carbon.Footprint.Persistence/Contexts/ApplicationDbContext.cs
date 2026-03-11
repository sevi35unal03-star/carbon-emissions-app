

namespace IzTek.Carbon.Footprint.Persistence.Contexts;

public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>, IApplicationDbContext
{
    private readonly IMessageBus _bus;
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IMessageBus bus,
        ICurrentUserService currentUserService) : base(options)
    {
        _bus = bus;
        _currentUserService = currentUserService;
    }

    // IdentityDbContext zaten Users ve Roles DbSet'lerini sağlıyor — tekrar tanımlamaya gerek yok
    // Product kaldırıldı — bu projede yok

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(
            new DispatchDomainEventsInterceptor(_bus),
            new AuditableEntityInterceptor(_currentUserService));
        base.OnConfiguring(optionsBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}