using IzTek.Carbon.Footprint.Domain.Events.Poll;

namespace IzTek.Carbon.Footprint.Domain.Entities;

public class UserPollResult : BaseAuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Surname { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public Guid PollSetId { get; private set; }
    public double TotalScore { get; private set; }
    public int TreeCount { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }

    public bool IsCompleted { get; private set; } = false;

    public void Complete() => IsCompleted = true;

    private readonly List<UserPollAnswer> _answers = new();
    public IReadOnlyCollection<UserPollAnswer> Answers => _answers.AsReadOnly();

    private UserPollResult()
    { }

    public UserPollResult(
        string name,
        string surname,
        Guid userId,
        Guid pollSetId,
        double totalScore,
        int treeCount)
    {
        Name = name;
        Surname = surname;
        UserId = userId;
        PollSetId = pollSetId;
        TotalScore = totalScore;
        TreeCount = treeCount;
        Month = DateTime.UtcNow.Month;
        Year = DateTime.UtcNow.Year;

        AddDomainEvent(new PollAnsweredDomainEvent(
            userId,
            pollSetId,
            totalScore,
            treeCount,
            Month,
            Year));
    }

    public void AddAnswer(
        Guid pollQuestionId,
        Guid pollOptionId,
        string questionText,
        string selectedOptionText,
        double carbonValue)
    {
        _answers.Add(new UserPollAnswer(
            userPollResultId: Id,
            pollQuestionId: pollQuestionId,
            pollOptionId: pollOptionId,
            questionText: questionText,
            selectedOptionText: selectedOptionText,
            carbonValue: carbonValue));
    }

    public void UpdateDraft(
    double totalScore,
    int treeCount,
    List<(Guid questionId, Guid optionId, string questionText, string optionText, double carbonValue)> answers)
    {
        TotalScore = totalScore;
        TreeCount = treeCount;
        _answers.Clear();
        foreach (var a in answers)
            _answers.Add(new UserPollAnswer(
                userPollResultId: Id,
                pollQuestionId: a.questionId,
                pollOptionId: a.optionId,
                questionText: a.questionText,
                selectedOptionText: a.optionText,
                carbonValue: a.carbonValue));
    }
}