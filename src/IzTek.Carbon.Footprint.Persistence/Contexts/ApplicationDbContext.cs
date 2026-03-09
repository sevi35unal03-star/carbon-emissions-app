using System.Reflection;
using Iztek.Carbon.Footprint.Domain.Entities;
using IzTek.Carbon.Footprint.Persistence.Interceptors; // Interceptor namespace'iniz
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wolverine;

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

    public DbSet<Product> Products => Set<Product>();
    public DbSet<UserAnswer> UserAnswers => Set<UserAnswer>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ActivityQuestion> ActivityQuestions => Set<ActivityQuestion>();
    public DbSet<ActivityOption> ActivityOptions => Set<ActivityOption>();
    public DbSet<UserActivityLog> UserActivityLogs => Set<UserActivityLog>();
    public DbSet<UsefulInformation> UsefulInformations => Set<UsefulInformation>();
    public DbSet<PollOption> PollOptions => Set<PollOption>();
    public DbSet<PollSet> PollSets => Set<PollSet>();
    public DbSet<PollQuestion> PollQuestions => Set<PollQuestion>();
    public DbSet<TreeDefinition> TreeDefinitions => Set<TreeDefinition>();
    public DbSet<UserPollResult> UserPollResults => Set<UserPollResult>();

    DbSet<UserAnswer> IApplicationDbContext.UserAnswers { get => UserAnswers; set => throw new NotImplementedException(); }
    DbSet<ActivityQuestion> IApplicationDbContext.ActivityQuestions { get => ActivityQuestions; set => throw new NotImplementedException(); }
    DbSet<ActivityOption> IApplicationDbContext.ActivityOptions { get => ActivityOptions; set => throw new NotImplementedException(); }
    DbSet<UserActivityLog> IApplicationDbContext.UserActivityLogs { get => UserActivityLogs; set => throw new NotImplementedException(); }
    DbSet<UsefulInformation> IApplicationDbContext.UsefulInformations { get => UsefulInformations; set => throw new NotImplementedException(); }
    DbSet<PollOption> IApplicationDbContext.PollOptions { get => PollOptions; set => throw new NotImplementedException(); }
    DbSet<PollQuestion> IApplicationDbContext.PollQuestions { get => PollQuestions; set => throw new NotImplementedException(); }
    DbSet<TreeDefinition> IApplicationDbContext.TreeDefinitions { get => TreeDefinitions; set => throw new NotImplementedException(); }
    DbSet<UserPollResult> IApplicationDbContext.UserPollResults { get => UserPollResults; set => throw new NotImplementedException(); }
    public DbSet<ScoringSetting> ScoringSettings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. ÖNEMLİ: builder değil, parametre olarak gelen modelBuilder kullanılmalı.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);

        // ActivityOption Hiyerarşi Yapılandırması
        //ParentOption yok!!
        modelBuilder.Entity<ActivityOption>(entity =>
        {
            entity.HasOne(x => x.ParentOption)
                .WithMany(x => x.SubOptions)
                .HasForeignKey(x => x.ParentOptionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Question)
                .WithMany(x => x.Options)
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserActivityLog Yapılandırması (Entity içindeki property isimleriyle uyumlu olmalı)
        modelBuilder.Entity<UserActivityLog>(entity =>
        {
            entity.Property(x => x.UserId).IsRequired();
            // Not: Eğer entity içinde isimler farklıysa (örn: TotalCarbonScore) burayı ona göre güncelleyin.
            entity.Property(x => x.TotalCarbonScore).IsRequired();
            entity.Property(x => x.ActivityDate).IsRequired();
        });

        modelBuilder.Entity<Role>().HasQueryFilter(r => !r.IsDeleted);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Interceptor'ları burada eklemek, bağımlılıkları yönetmeyi zorlaştırabilir.
        // Genelde Program.cs tarafında eklenmesi tercih edilir ancak burada kalacaksa alttaki gibi olmalı:
        optionsBuilder.AddInterceptors(
            new DispatchDomainEventsInterceptor(_bus),
            new AuditableEntityInterceptor(_currentUserService));

        base.OnConfiguring(optionsBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // SaveChanges tetiklenmeden önce yapılacak işlemler interceptor'lar tarafından yönetiliyor.
        return await base.SaveChangesAsync(cancellationToken);
    }
}