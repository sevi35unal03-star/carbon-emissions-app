namespace IzTek.Carbon.Footprint.Domain.Entities;

public class UserPollAnswer : BaseAuditableEntity
{
    public Guid UserPollResultId { get; private set; }
    public UserPollResult UserPollResult { get; private set; } = null!;

    public Guid PollQuestionId { get; private set; }
    public Guid PollOptionId { get; private set; }

    // Snapshot — soru/seçenek sonradan değişse bile cevap korunur
    public string QuestionText { get; private set; } = null!;
    public string SelectedOptionText { get; private set; } = null!;
    public double CarbonValue { get; private set; }

    private UserPollAnswer() { }

    public UserPollAnswer(
        Guid userPollResultId,
        Guid pollQuestionId,
        Guid pollOptionId,
        string questionText,
        string selectedOptionText,
        double carbonValue)
    {
        UserPollResultId = userPollResultId;
        PollQuestionId = pollQuestionId;
        PollOptionId = pollOptionId;
        QuestionText = questionText;
        SelectedOptionText = selectedOptionText;
        CarbonValue = carbonValue;
    }
}