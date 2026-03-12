namespace IzTek.Carbon.Footprint.Application.Common.Validators;

/// <summary>
/// Email validation utilities using multiple validation strategies
/// </summary>
public static partial class EmailValidator
{
    private const string EmailPattern =
        @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

    [GeneratedRegex(EmailPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    /// <summary>
    /// Validates email using .NET EmailAddressAttribute
    /// </summary>
    /// <param name="email">Email address to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool IsValid([NotNullWhen(true)] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var validator = new EmailAddressAttribute();
        return validator.IsValid(email);
    }

    /// <summary>
    /// Validates email using RFC 5322 regex pattern
    /// Use when you need strict RFC compliance
    /// </summary>
    public static bool IsValidRegex([NotNullWhen(true)] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex().IsMatch(email);
    }

    /// <summary>
    /// Validates email using MailAddress parsing
    /// More permissive, use for parsing display names
    /// </summary>
    public static bool IsValidMailAddress(
        [NotNullWhen(true)] string email,
        [NotNullWhen(true)] out MailAddress? mailAddress)
    {
        mailAddress = null;

        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            mailAddress = new MailAddress(email);
            return mailAddress.Address == email.Trim();
        }
        catch (FormatException)
        {
            return false;
        }
    }

    /// <summary>
    /// Quick validation for basic email format (local@domain.tld)
    /// Use for performance-critical scenarios
    /// </summary>
    public static bool IsValidBasic([NotNullWhen(true)] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var atIndex = email.IndexOf('@');
        if (atIndex <= 0 || atIndex == email.Length - 1)
            return false;

        var dotIndex = email.LastIndexOf('.');
        return dotIndex > atIndex && dotIndex < email.Length - 1;
    }
}