using Iztek.Carbon.Footprint.Domain.Entities;

namespace IzTek.Carbon.Footprint.Domain.Entities;

public class ActivityOption : BaseAuditableEntity
{
    // private set -> Encapsulation
    // dışarıdan doğrudan değiştirilemez.
    public string Text { get; private set; } = null!;
    public double CarbonValue { get; private set; }
    public Guid ActivityQuestionId { get; private set; }
    public ActivityQuestion ActivityQuestion { get; private set; } = null!;

    /// <summary>
    ///     Soru 1 → Seçenek A → NextQuestionId var  → Soru 2'ye geç
    //      Soru 1 → Seçenek B → NextQuestionId null → Flow bitti
    /// </summary>
    public Guid? NextQuestionId { get; private set; }
    public ActivityQuestion? NextQuestion { get; private set; }
    public int DisplayOrder { get; internal set; }

    /// <summary>
    /// EF Core, veritabanından veri çekerken parametresiz constructor ile instance oluşturur.
    /// private veya protected yapılır ki dışarıdan new ActivityOption() ile boş nesne oluşturulmasın.
    /// </summary>
    private ActivityOption() { }  // ✅ protected → private (EF Core private constructor'ı destekler)

    public ActivityOption(string text, double carbonValue, Guid activityQuestionId, Guid? nextQuestionId = null)
    {
        ActivityQuestionId = activityQuestionId;
        UpdateDetails(text, carbonValue, nextQuestionId);
   
    }

    /// <summary>
    /// Hem constructor hem de güncelleme aynı validasyon logic'ini kullanıyor. 
    /// Tekrar yazmamak için ortak metot çıkarılmış
    /// </summary>
    public void UpdateDetails(string text, double carbonValue, Guid? nextQuestionId = null)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Seçenek metni boş olamaz.");

        Text = text;
        CarbonValue = carbonValue;
        NextQuestionId = nextQuestionId;
    }
}

/*
 private set      → Dışarıdan değiştirilemez (Encapsulation)
protected()      → EF Core kullanabilir, dışarıdan new'lenemez  
UpdateDetails()  → Validasyon tek yerde, constructor da kullanır
Guid? Next       → Soru akışı için opsiyonel zincir
default!         → Null safety uyarısını bastırır*/