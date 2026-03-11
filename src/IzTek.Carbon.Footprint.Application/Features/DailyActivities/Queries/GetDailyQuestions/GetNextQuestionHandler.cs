namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyQuestions;

    public record GetNextQuestionQuery(
        Guid QuestionId,
        Guid SelectedOptionId,
        Guid UserId
    );

    public static class GetNextQuestionHandler
    {
        public static async Task<DailyQuestionResponse?> Handle(
            GetNextQuestionQuery query,
            IApplicationDbContext context,
            CancellationToken ct)
        {
            // Gelen QuestionId'ye göre soruyu ve seçeneklerini getiriyoruz
            var question = await context.ActivityQuestions
                .AsNoTracking()
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == query.QuestionId, ct);

            if (question == null) return null;

            return new DailyQuestionResponse(
                question.Id,
                question.Text,
                question.DisplayOrder,
                question.Options.Select(o => new DailyOptionResponse(
                    o.Id,
                    o.Text,
                    o.CarbonValue,
                    o.NextQuestionId
                )).ToList()
            );
        }
    }
