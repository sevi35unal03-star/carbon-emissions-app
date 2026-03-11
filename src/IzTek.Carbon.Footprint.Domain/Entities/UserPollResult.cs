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

    private readonly List<UserPollAnswer> _answers = new();
    public IReadOnlyCollection<UserPollAnswer> Answers => _answers.AsReadOnly();

    private UserPollResult() { }

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
}