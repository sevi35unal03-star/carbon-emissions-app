namespace IzTek.Carbon.Footprint.Domain.Entities;

public class PollOption : BaseAuditableEntity
{
    public Guid PollQuestionId { get; private set; }
    public string Text { get; private set; } = null!;
    public double CarbonValue { get; private set; } // TotalCarbonScore hesaplaması için
    public Guid? NextPollQuestionId { get; private set; }
    public int DisplayOrder { get; private set; }

    // Navigation property
    public PollQuestion PollQuestion { get; private set; } = null!;

    private PollOption() { }

    public PollOption(Guid pollQuestionId, string text, double carbonValue, Guid? nextPollQuestionId = null, int displayOrder = 0)
    {
        PollQuestionId = pollQuestionId; 
        Text = text;
        CarbonValue = carbonValue;
        NextPollQuestionId = nextPollQuestionId; 
        DisplayOrder = displayOrder;
    }

    public static PollOption CloneFrom(ActivityOption source, Guid pollQuestionId)
    {
        return new PollOption(
            pollQuestionId: pollQuestionId,
            text: source.Text,
            carbonValue: source.CarbonValue,
            nextPollQuestionId: null,
            displayOrder: source.DisplayOrder
        );
    }

    public void UpdateDetails(string text, double carbonValue, Guid? nextPollQuestionId, int displayOrder)
    {
        Text = text;
        CarbonValue = carbonValue;
        NextPollQuestionId = nextPollQuestionId;
        DisplayOrder = displayOrder;
    }
}