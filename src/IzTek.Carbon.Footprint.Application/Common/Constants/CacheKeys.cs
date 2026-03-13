namespace IzTek.Carbon.Footprint.Application.Common.Constants;

public static class CacheKeys
{
    // ─── Scoring ──────────────────────────────────────────────────────────────
    public static class ScoringSettings
    {
        public const string All = "scoring-settings:all";
        public const string Prefix = "scoring-settings:";
    }

    public static string GlobalScoringParams => "scoring-settings:global";

    // ─── Tree ─────────────────────────────────────────────────────────────────
    public static class TreeDefinition
    {
        public const string Active = "tree-definition:active";
        public const string Ratio = "tree-definition:ratio";
        public const string Prefix = "tree-definition:";
    }

    // ─── Activity ─────────────────────────────────────────────────────────────
    public static class ActivityQuestions
    {
        public const string Prefix = "activity-questions:";
        public static string ById(Guid id) => $"activity-questions:{id}";
    }

    // ─── Poll ─────────────────────────────────────────────────────────────────
    public static class Poll
    {
        public const string ActiveMonthly = "poll:active-monthly";
        public const string Prefix = "poll:";
    }

    // ─── Goals ────────────────────────────────────────────────────────────────
    public static class Goals
    {
        public static string Detail(int month, int year) => $"goal-detail:{month}:{year}";
        public static string Yearly(int year) => $"yearly-goals:{year}";
        public const string Prefix = "goal:";
    }

    // ─── Leaderboard ──────────────────────────────────────────────────────────
    public static class Leaderboard
    {
        public static string Monthly(int month, int year) => $"leaderboard:monthly:{year}:{month:D2}";
        public const string Prefix = "leaderboard:";
    }

    // ─── User ─────────────────────────────────────────────────────────────────
    public static class User
    {
        public static string DonationHistory(Guid userId) => $"user:donation-history:{userId}";
        public const string Prefix = "user:";
    }

    // ─── Useful Information ───────────────────────────────────────────────────
    public static class UsefulInformation
    {
        public const string List = "useful-information:list";
        public const string Prefix = "useful-information:";
    }
}