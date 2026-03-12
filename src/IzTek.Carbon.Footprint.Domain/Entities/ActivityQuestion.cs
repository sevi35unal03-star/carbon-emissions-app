namespace IzTek.Carbon.Footprint.Domain.Entities;

public class ActivityQuestion : BaseAuditableEntity
{
    public string Text { get; private set; } = null!;
    public int DisplayOrder { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public TimeSpan ScheduledTime { get; private set; }

    private readonly List<ActivityOption> _options = [];
    public IReadOnlyCollection<ActivityOption> Options => _options;


    private ActivityQuestion() { }

    public ActivityQuestion(string text, TimeSpan scheduledTime, int displayOrder, DateTime startDate, DateTime endDate)
    {
        UpdateDetails(text, displayOrder, startDate, endDate, scheduledTime);
    }

    public void UpdateDetails(string text, int displayOrder, DateTime startDate, DateTime endDate, TimeSpan scheduledTime)
    {
        if (endDate < startDate)
            throw new ArgumentException("Bitiş tarihi başlangıçtan önce olamaz.");
        Text = text;
        DisplayOrder = displayOrder;
        StartDate = startDate;
        EndDate = endDate;
        ScheduledTime = scheduledTime;
    }

    public void AddOption(string text, double carbonValue, Guid? nextQuestionId = null)
    {
        // PollQuestionId → Id (düzeltildi)
        var option = new ActivityOption(text, carbonValue, Id, nextQuestionId);
        _options.Add(option);
    }
}