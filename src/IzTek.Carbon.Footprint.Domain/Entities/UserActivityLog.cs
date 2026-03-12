namespace IzTek.Carbon.Footprint.Domain.Entities;

/// <summary>
/// Kullanıcının aktivite cevap log kaydı. Audit ve raporlama amaçlı.
/// Puan güncelleme sorumluluğu burada değil — SubmitActivityAnswerCommandHandler'da.
/// </summary>
public class UserActivityLog : BaseAuditableEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public Guid ActivityQuestionId { get; private set; }
    public ActivityQuestion ActivityQuestion { get; private set; } = null!;
    public Guid ActivityOptionId { get; private set; }
    public ActivityOption ActivityOption { get; private set; } = null!;
    public double TotalCarbonScore { get; private set; }
    public DateTime ActivityDate { get; private set; }

    public Guid SelectedOptionId { get; private set; }
    public string SelectedOptionText { get; private set; } = null!;
    public double CarbonValue { get; private set; }

    private UserActivityLog() { }

    public UserActivityLog(Guid userId, Guid questionId, Guid optionId, double score, Guid selectedOptionId, string selectedOptionText, double carbonValue)
    {
        UserId = userId;
        ActivityQuestionId = questionId;
        ActivityOptionId = optionId;
        TotalCarbonScore = score;
        ActivityDate = DateTime.UtcNow;
        SelectedOptionId = selectedOptionId;
        SelectedOptionText = selectedOptionText;
        CarbonValue = carbonValue;
    }
}