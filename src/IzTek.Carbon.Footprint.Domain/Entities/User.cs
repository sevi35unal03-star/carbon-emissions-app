

namespace IzTek.Carbon.Footprint.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string? Name { get; private set; }
    public string? Surname { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public string? IdentityNumber { get; private set; }
    public bool IsKvkkApproved { get; private set; } = false;
    public DateTime? KvkkApprovalDate { get; private set; }
    public double TotalPoints { get; private set; }         // Biriktirilen toplam puan — liderboard + profil
    public double LastCarbonScore { get; private set; }     // Son anket puanı — profil ekranı
    public int DonatedTreeCount { get; private set; }       // Toplam bağışlanan ağaç — Bağışlarım
    public DateTime? LastDonationDate { get; private set; } // Son bağış tarihi
    public DateTime? LastLoginDate { get; private set; }    // Log kayıtları için
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedDate { get; private set; }

    // Domain Events — IdentityUser'da olmadığı için manuel ekliyoruz
    private readonly List<BaseEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void AddDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();

    private User() { }

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
    public void DonateAllPoints(int treeCount)
    {
        DonatedTreeCount += treeCount;
        LastDonationDate = DateTime.UtcNow;
        TotalPoints = 0;
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
    }
}