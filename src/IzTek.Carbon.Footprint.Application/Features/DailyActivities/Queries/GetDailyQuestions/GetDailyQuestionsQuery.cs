public record DailyOptionResponse(
    Guid Id,
    string Text,
    double CarbonValue, // Entity ile aynı isim
    Guid? NextQuestionId // Kırılımın anahtarı bu!
);

public record DailyQuestionResponse(
    Guid Id,
    string Text,
    int DisplayOrder,
    List<DailyOptionResponse> Options
);