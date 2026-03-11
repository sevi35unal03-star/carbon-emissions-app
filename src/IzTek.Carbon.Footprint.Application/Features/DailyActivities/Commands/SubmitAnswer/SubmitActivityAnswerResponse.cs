using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyQuestions;

namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Commands.SubmitAnswer;

public class SubmitActivityAnswerResponse
{
    // Bir sonraki soru — flow bitmemişse dolu, bitmişse null
    public DailyQuestionResponse? NextQuestion { get; set; }

    // Kullanıcının bugünkü toplam karbon skoru
    public double TotalCarbonScore { get; set; }

    // Tüm sorular cevaplandıysa true
    public bool IsFlowCompleted { get; set; }
}