namespace IzTek.Carbon.Footprint.Application.Common.Validators;

/// <summary>
/// Identity document validation utilities for Turkish documents
/// </summary>
public static partial class IdentityValidator
{
    /// <summary>
    /// Validates Turkish Identity Number (TC Kimlik No) using the official algorithm
    /// <para>Algorithm: 11 digits, 1st digit != 0, checksum validation</para>
    /// <para>10th digit = ((sum of odd positions * 7) - (sum of even positions)) % 10</para>
    /// <para>11th digit = (sum of first 10 digits) % 10</para>
    /// </summary>
    /// <param name="identityNumber">11-digit Turkish identity number</param>
    /// <returns>True if valid according to the official algorithm</returns>
    public static bool IsValidTurkishIdentityNumber([NotNullWhen(true)] string identityNumber)
    {
        if (string.IsNullOrWhiteSpace(identityNumber))
            return false;

        if (identityNumber.Length != 11)
            return false;

        if (!identityNumber.All(char.IsDigit))
            return false;

        if (identityNumber[0] == '0')
            return false;

        var digits = identityNumber.Select(c => c - '0').ToArray();

        var oddSum = 0;
        var evenSum = 0;

        for (int i = 0; i < 9; i++)
        {
            if (i % 2 == 0)
                oddSum += digits[i];
            else
                evenSum += digits[i];
        }

        var digit10 = ((oddSum * 7) - evenSum) % 10;
        if (digit10 < 0) digit10 += 10;

        if (digits[9] != digit10)
            return false;

        var digit11 = digits.Take(10).Sum() % 10;

        return digits[10] == digit11;
    }

    /// <summary>
    /// Passport validation patterns for different formats
    /// </summary>
    private static readonly PassportPattern[] PassportPatterns =
    [
        new("Numeric", @"^\d{6,12}$"),
        new("Alphanumeric", @"^[A-Z\d<]{6,12}$"),
        new("AlphaPrefix", @"^[A-Z]{1,3}\d{5,12}$"),
        new("AlphaSuffix", @"^\d{5,12}[A-Z]{1,3}$"),
        new("AlphaPrefixSuffix", @"^[A-Z]{1,3}\d{5,12}[A-Z]{1,2}$"),
        new("MixedRequired", @"^(?=.*[A-Z])(?=.*\d)[A-Z\d]{6,12}$")
    ];

    [GeneratedRegex(@"^\d{6,12}$", RegexOptions.Compiled)]
    private static partial Regex NumericPassportRegex();

    [GeneratedRegex(@"^[A-Z\d<]{6,12}$", RegexOptions.Compiled)]
    private static partial Regex AlphanumericPassportRegex();

    [GeneratedRegex(@"^[A-Z]{1,3}\d{5,12}$", RegexOptions.Compiled)]
    private static partial Regex AlphaPrefixPassportRegex();

    [GeneratedRegex(@"^\d{5,12}[A-Z]{1,3}$", RegexOptions.Compiled)]
    private static partial Regex AlphaSuffixPassportRegex();

    [GeneratedRegex(@"^[A-Z]{1,3}\d{5,12}[A-Z]{1,2}$", RegexOptions.Compiled)]
    private static partial Regex AlphaPrefixSuffixPassportRegex();

    [GeneratedRegex(@"^(?=.*[A-Z])(?=.*\d)[A-Z\d]{6,12}$", RegexOptions.Compiled)]
    private static partial Regex MixedRequiredPassportRegex();

    /// <summary>
    /// Validates passport number against multiple common international formats
    /// <para>Supports various formats: numeric, alphanumeric, prefix/suffix patterns</para>
    /// <para>Note: This is a generic validation. Use country-specific methods for strict validation</para>
    /// </summary>
    /// <param name="passportNumber">Passport number to validate</param>
    /// <returns>True if matches any common passport format</returns>
    public static bool IsValidPassport([NotNullWhen(true)] string? passportNumber)
    {
        if (string.IsNullOrWhiteSpace(passportNumber))
            return false;

        var normalized = passportNumber.Trim().ToUpperInvariant();

        return NumericPassportRegex().IsMatch(normalized) ||
               AlphanumericPassportRegex().IsMatch(normalized) ||
               AlphaPrefixPassportRegex().IsMatch(normalized) ||
               AlphaSuffixPassportRegex().IsMatch(normalized) ||
               AlphaPrefixSuffixPassportRegex().IsMatch(normalized) ||
               MixedRequiredPassportRegex().IsMatch(normalized);
    }

    /// <summary>
    /// Validates Turkish passport number (Format: U12345678 - Letter + 8 digits)
    /// </summary>
    [GeneratedRegex(@"^[A-Z]\d{8}$", RegexOptions.Compiled)]
    private static partial Regex TurkishPassportRegex();

    public static bool IsValidTurkishPassport([NotNullWhen(true)] string passportNumber)
    {
        if (string.IsNullOrWhiteSpace(passportNumber))
            return false;

        var normalized = passportNumber.Trim().ToUpperInvariant();
        return TurkishPassportRegex().IsMatch(normalized);
    }

    /// <summary>
    /// Validates passport number for a specific country
    /// </summary>
    public static bool IsValidPassport([NotNullWhen(true)] string passportNumber, PassportCountry country)
    {
        return country switch
        {
            PassportCountry.Turkey => IsValidTurkishPassport(passportNumber),
            PassportCountry.Generic => IsValidPassport(passportNumber),
            _ => throw new ArgumentException($"Unsupported country: {country}")
        };
    }

    public enum PassportCountry
    {
        Generic,
        Turkey
        // Add more countries as needed
    }

    internal sealed record PassportPattern(string Name, string Pattern);
}