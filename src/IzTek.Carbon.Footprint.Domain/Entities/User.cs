namespace IzTek.Carbon.Footprint.Domain.Entities;

public class User : BaseEntity
{
    public bool EmailConfirmed {  get; private set; }

    public  string Email { get; private set; }

    public string? Name { get; private set; }
    public string? Surname { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public string? IdentityNumber { get; private set; }
    public string? PhoneNumber { get; private set; }

    public string? Password { get; private set; }
    public string? ConfirmPassword { get; private set; }
    public bool IsKvkkApproved { get; private set; } = false;
    public DateTime? KvkkApprovalDate { get; private set; }
    public double LastCarbonScore { get; private set; }
    public double TotalPoints { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedDate { get; private set; }
    public string UserName { get; private set; }
    public double TotalCarbonScore { get; private set; }
    public DateTime LastLoginDate { get; private set; }
    public double TotalCarbonPoint { get; private set; }

    private User () { }

    public User(bool emailConfirmed,
        string email,
        string? name,
        string? surname,
        DateTime? birthDate,
        string? identityNumber,
        string? phoneNumber,
        string? password,
        string? confirmPassword,
        bool isKvkkApproved,
        DateTime? kvkkApprovalDate,
        double lastCarbonScore,
        double totalPoints,
        bool isDeleted,
        DateTime? deletedDate,
        string userName,
        double totalCarbonScore,
        DateTime lastLoginDate,
        double totalCarbonPoint)
    {
        EmailConfirmed = emailConfirmed;
        Email = email;
        Name = name;
        Surname = surname;
        BirthDate = birthDate;
        IdentityNumber = identityNumber;
        PhoneNumber = phoneNumber;
        Password = password;
        ConfirmPassword = confirmPassword;
        IsKvkkApproved = isKvkkApproved;
        KvkkApprovalDate = kvkkApprovalDate;
        LastCarbonScore = lastCarbonScore;
        TotalPoints = totalPoints;
        IsDeleted = isDeleted;
        DeletedDate = deletedDate;
        UserName = userName;
        TotalCarbonScore = totalCarbonScore;
        LastLoginDate = lastLoginDate;
        TotalCarbonPoint = totalCarbonPoint;
    }

    public void UpdateMonthlyCarbonResult(double pollScore, int treeCount)
    {
        // Son doldurduğu anketin puanı
        this.LastCarbonScore = pollScore;

        // Toplam puanına ekle (Eğer kurgun bu yöndeyse)
        this.TotalPoints += pollScore;
    }

}

//UpdateMonthlyCarbonResult ekle



