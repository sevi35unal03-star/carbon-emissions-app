using IzTek.Carbon.Footprint.Domain.Entities;

namespace Iztek.Carbon.Footprint.Domain.Entities;

public class ActivityQuestion : BaseAuditableEntity
{
    public string Text { get; private set; } = default!;
    public int DisplayOrder { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    // Sorunun gün içindeki aktiflik saati (Örn: 08:00)
    public TimeSpan ScheduledTime { get; private set; }

    private readonly List<ActivityOption> _options = new();
    public IReadOnlyCollection<ActivityOption> Options => _options;

    // Kök soru mu yoksa bir seçeneğe mi bağlı?
    public bool IsRoot => ParentOptionId == null;
    public Guid? ParentOptionId { get; private set; }
    public ActivityOption? ParentOption { get; private set; }

    private ActivityQuestion() { }

    public ActivityQuestion(string text, TimeSpan scheduledTime, int displayOrder, DateTime startDate, DateTime endDate, Guid? parentOptionId = null)
    {
        ParentOptionId = parentOptionId;
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
        // Encapsulation: Seçenek ekleme kuralı buraya gelebilir
        var option = new ActivityOption(text, carbonValue, PollQuestionId, nextQuestionId);
        _options.Add(option);
    }
}