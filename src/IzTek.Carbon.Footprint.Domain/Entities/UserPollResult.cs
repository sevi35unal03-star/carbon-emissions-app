//UserId, PollSetId, TotalScore, TreeCount, Month, Year

namespace IzTek.Carbon.Footprint.Domain.Entities;

public class UserPollResult : BaseAuditableEntity
{
    public string Name { get; private set; }

    public string Surname { get; private set; }
    public Guid UserId { get; private set; }
    public Guid PollSetId { get; private set; }
    public double TotalScore { get; private set; } // O ayki anket puanı
    public int TreeCount { get; private set; }    // O puanın karşılığı olan ağaç
    public int Month { get; private set; }        // İşlemin ayı (1-12)
    public int Year { get; private set; }         // İşlemin yılı (2024, 2025 vb.)

    private UserPollResult() { } 

    public UserPollResult(string name, string surname, Guid userId, Guid pollSetId, double totalScore, int treeCount)
    {
        Name = name;
        Surname = surname;
        UserId = userId;
        PollSetId = pollSetId;
        TotalScore = totalScore;
        TreeCount = treeCount;
        Month = DateTime.UtcNow.Month;
        Year = DateTime.UtcNow.Year;
    }
}