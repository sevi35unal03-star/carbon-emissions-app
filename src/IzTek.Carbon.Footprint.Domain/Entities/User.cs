using IzTek.Carbon.Footprint.Domain.Events.User;
using IzTek.Carbon.Footprint.Domain.Common.Exceptions;

namespace IzTek.Carbon.Footprint.Domain.Entities;

public class User : IdentityUser<Guid>, IDomainEventContainer
{
    public string? Name { get; private set; } = null!;
    public string? Surname { get; private set; } = null!;
    public DateTime? BirthDate { get; private set; } = DateTime.UtcNow;
    public string? IdentityNumber { get; private set; } = null!;
    public bool IsKvkkApproved { get; private set; } = false;
    public DateTime? KvkkApprovalDate { get; private set; } = DateTime.UtcNow;
    public double TotalPoints { get; private set; }         // Biriktirilen toplam puan — liderboard + profil
    public double LastCarbonScore { get; private set; }     // Son anket puanı — profil ekranı
    public int DonatedTreeCount { get; private set; }       // Toplam bağışlanan ağaç — Bağışlarım
    public DateTime? LastDonationDate { get; private set; } = DateTime.UtcNow; // Son bağış tarihi
    public DateTime? LastLoginDate { get; private set; } = DateTime.UtcNow;  // Log kayıtları için
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedDate { get; private set; } = DateTime.UtcNow;

    // Domain Events — IdentityUser'da olmadığı için manuel ekliyoruz
    private readonly List<BaseEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    private User()
    { }

    public User(
        string email,
        string? name,
        string? surname,
        DateTime? birthDate,
        string? identityNumber,
        string? phoneNumber,
        bool isKvkkApproved)
    {
        Email = email;
        UserName = email; // BizİZmir'de email = username
        PhoneNumber = phoneNumber;
        Name = name;
        Surname = surname;
        BirthDate = birthDate;
        IdentityNumber = identityNumber;
        IsKvkkApproved = isKvkkApproved;
        KvkkApprovalDate = isKvkkApproved ? DateTime.UtcNow : null;
        EmailConfirmed = false;

        AddDomainEvent(new UserRegisteredDomainEvent(Id, email, $"{name} {surname}"));
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
    }

    /// <summary>
    /// Aylık anket tamamlandığında çağrılır.
    /// Profil ekranı ve liderboard için TotalPoints ve LastCarbonScore güncellenir.
    /// </summary>
    public void UpdateMonthlyCarbonResult(double pollScore)
    {
        LastCarbonScore = pollScore;
        TotalPoints += pollScore;
    }

    /// <summary>
    /// Kullanıcı ağaç bağışı yaptığında çağrılır.
    /// Puanlar sıfırlanır, ağaç sayısı artar.
    /// </summary>
    /// <summary>
    /// Kullanıcı belirli miktarda puan bağışlar.
    /// Yeterli puan yoksa Result.Failure döner.
    /// </summary>
    /// <summary>
    /// Kullanıcı belirli miktarda puan bağışlar.
    /// Yeterli puan yoksa Result.Failure döner.
    /// </summary>
    /// <summary>
    /// Kullanıcı belirli miktarda puan bağışlar.
    /// Yeterli puan yoksa DomainException fırlatır.
    /// </summary>
    public void DonatePoints(double points, int treeCount)
    {
        if (points <= 0)
            throw new DomainException("Bağış miktarı pozitif olmalıdır.");

        if (TotalPoints < points)
            throw new DomainException("Yetersiz puan.");

        TotalPoints -= points;
        DonatedTreeCount += treeCount;
        LastDonationDate = DateTime.UtcNow;

        AddDomainEvent(new TreesDonatedDomainEvent(Id, treeCount, points, DateTime.UtcNow));
    }

    /// <summary>
    /// Login olduğunda çağrılır — log kayıtları için.
    /// </summary>
    public void UpdateLastLoginDate()
    {
        LastLoginDate = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedDate = DateTime.UtcNow;
        AddDomainEvent(new UserDeletedDomainEvent(Id, DeletedDate.Value));
    }
}