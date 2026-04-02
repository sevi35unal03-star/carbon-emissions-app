namespace IzTek.Carbon.Footprint.Persistence.Interceptors;

public class AuditInterceptor(ICurrentUserService currentUser) : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser = currentUser;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        OnBeforeSaveChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        OnBeforeSaveChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static readonly HashSet<string> SensitiveFields = new(StringComparer.OrdinalIgnoreCase)
{
    "PasswordHash",
    "SecurityStamp",
    "ConcurrencyStamp",
    "IdentityNumber",
    "PhoneNumber",
    "Token",           // RefreshToken
    "RevokedReason",
    "NormalizedEmail",
    "NormalizedUserName",
    "TwoFactorEnabled",
    "LockoutEnd",
    "AccessFailedCount"
};

    private void OnBeforeSaveChanges(DbContext? context)
    {
        if (context is null) return;
        context.ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Entity.GetType().Name,
                UserId = _currentUser.UserId?.ToString() ?? "system",
                UserName = _currentUser.UserName ?? "system",
            };
            auditEntries.Add(auditEntry);

            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey()) continue;

                // Hassas alanları loglaма
                if (SensitiveFields.Contains(propertyName))
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.Operation = "Ekleme";
                            auditEntry.NewValues[propertyName] = "[REDACTED]";
                            break;

                        case EntityState.Modified when property.IsModified:
                            auditEntry.Operation = "Güncelleme";
                            auditEntry.OldValues[propertyName] = "[REDACTED]";
                            auditEntry.NewValues[propertyName] = "[REDACTED]";
                            break;
                    }
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.Operation = "Ekleme";
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        auditEntry.Operation = "Silme";
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.Operation = "Güncelleme";
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }
        }

        foreach (var auditEntry in auditEntries)
            context.Set<AuditLog>().Add(auditEntry.ToAuditLog());
    }
}