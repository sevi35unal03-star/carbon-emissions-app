namespace IzTek.Carbon.Footprint.Application.Common.Constants;

public static class CacheKeys
{
    public static string GlobalScoringParams { get; internal set; } = default!;

    public static class TreeDefinition
    {
        public const string Active = "tree_definition:active";
        public const string Ratio = "tree_definition:ratio";
    }

    public static class ScoringSettings
    {
        public const string All = "scoring_settings:all";
        public const string Prefix = "scoring_settings:";
    }

    public static class ActivityQuestions
    {
        public const string Prefix = "activity_questions:";
        public static string ById(Guid id) => $"activity_questions:{id}";
    }
}