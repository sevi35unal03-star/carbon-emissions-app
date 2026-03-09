namespace IzTek.Carbon.Footprint.Application.Common.Validators;

/// <summary>
/// Phone number validation using Google's libphonenumber
/// </summary>
public static class PhoneNumberValidator
{
    private static readonly PhoneNumberUtil _phoneUtil = PhoneNumberUtil.GetInstance();

    /// <summary>
    /// Allowed country/region codes for phone number registration
    /// Primarily covers Europe and neighboring regions
    /// </summary>
    private static readonly HashSet<string> AllowedRegions =
    [
        "TR", // Turkey (primary)
        "DE", "GB", "FR", "IT", "ES", "NL", // Western Europe
        "PL", "CZ", "HU", "RO", "BG", "GR", // Central/Eastern Europe
        "NO", "SE", "DK", "FI", "IS", // Nordic
        "PT", "BE", "AT", "CH", "IE", // Others
        "HR", "SI", "SK", "EE", "LV", "LT", // Baltic/Balkan
        "CY", "MT", "LU", "AL", "BA", "RS", "ME", "MK", "XK", // Small states
        "RU", "UA", "BY", "MD", "GE", "AM", "AZ", "KZ", "IR", // Eastern neighbors
        "AD", "MC", "SM", "LI", "VA" // Microstates
    ];

    /// <summary>
    /// Validates international phone number format
    /// Accepts numbers with + prefix or international format
    /// </summary>
    /// <param name="phoneNumber">Phone number (e.g., "+905551234567")</param>
    /// <param name="parsedNumber">Parsed phone number object if valid</param>
    /// <returns>True if valid international phone number</returns>
    public static bool IsValid(
        [NotNullWhen(true)] string phoneNumber,
        [NotNullWhen(true)] out PhoneNumber parsedNumber)
    {
        parsedNumber = null;

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        try
        {
            parsedNumber = _phoneUtil.Parse(phoneNumber, null);
            return _phoneUtil.IsValidNumber(parsedNumber);
        }
        catch (NumberParseException)
        {
            return false;
        }
    }

    /// <summary>
    /// Validates international phone number format (simple overload)
    /// </summary>
    public static bool IsValid([NotNullWhen(true)] string phoneNumber)
        => IsValid(phoneNumber, out _);

    /// <summary>
    /// Validates phone number with default country context
    /// Useful for local numbers without country code
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="defaultRegion">Default region code (e.g., "TR")</param>
    /// <param name="parsedNumber">Parsed phone number object if valid</param>
    /// <returns>True if valid</returns>
    public static bool IsValidWithRegion(
        [NotNullWhen(true)] string phoneNumber,
        string defaultRegion,
        [NotNullWhen(true)] out PhoneNumber parsedNumber)
    {
        parsedNumber = null;

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        try
        {
            parsedNumber = _phoneUtil.Parse(phoneNumber, defaultRegion);
            return _phoneUtil.IsValidNumber(parsedNumber);
        }
        catch (NumberParseException)
        {
            return false;
        }
    }

    /// <summary>
    /// Validates if phone number is from an allowed region
    /// Checks against whitelist of supported countries
    /// </summary>
    public static bool IsAllowedRegion([NotNullWhen(true)] string phoneNumber)
    {
        if (!IsValid(phoneNumber, out var parsedNumber))
            return false;

        var region = _phoneUtil.GetRegionCodeForNumber(parsedNumber);

        return !string.IsNullOrEmpty(region) && AllowedRegions.Contains(region);
    }

    /// <summary>
    /// Validates if phone number is from an allowed region with parsed output
    /// </summary>
    public static bool IsAllowedRegion(
        [NotNullWhen(true)] string phoneNumber,
        [NotNullWhen(true)] out PhoneNumber parsedNumber,
        out string regionCode)
    {
        regionCode = null;

        if (!IsValid(phoneNumber, out parsedNumber))
            return false;

        regionCode = _phoneUtil.GetRegionCodeForNumber(parsedNumber);

        return !string.IsNullOrEmpty(regionCode) && AllowedRegions.Contains(regionCode);
    }

    /// <summary>
    /// Validates Turkish mobile phone number
    /// Accepts formats: "+905551234567", "905551234567", "05551234567", "5551234567"
    /// </summary>
    public static bool IsValidTurkishMobile([NotNullWhen(true)] string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Normalize: remove spaces, dashes, parentheses
        var normalized = new string(phoneNumber.Where(char.IsDigit).ToArray());

        // Try parsing with TR context
        if (!IsValidWithRegion(normalized, "TR", out var parsedNumber))
            return false;

        // Check if it's actually Turkish
        var region = _phoneUtil.GetRegionCodeForNumber(parsedNumber);
        if (region != "TR")
            return false;

        // Check if it's a mobile number
        var numberType = _phoneUtil.GetNumberType(parsedNumber);
        return numberType == PhoneNumberType.MOBILE ||
               numberType == PhoneNumberType.FIXED_LINE_OR_MOBILE;
    }

    /// <summary>
    /// Validates Turkish phone number (mobile or landline)
    /// </summary>
    public static bool IsValidTurkishNumber([NotNullWhen(true)] string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        var normalized = new string(phoneNumber.Where(char.IsDigit).ToArray());

        if (!IsValidWithRegion(normalized, "TR", out var parsedNumber))
            return false;

        var region = _phoneUtil.GetRegionCodeForNumber(parsedNumber);
        return region == "TR";
    }

    /// <summary>
    /// Formats phone number to E164 format (+905551234567)
    /// </summary>
    public static string? FormatE164([NotNullWhen(true)] string phoneNumber)
    {
        if (!IsValid(phoneNumber, out var parsedNumber))
            return null;

        return _phoneUtil.Format(parsedNumber, PhoneNumberFormat.E164);
    }

    /// <summary>
    /// Formats phone number to international format (+90 555 123 45 67)
    /// </summary>
    public static string? FormatInternational([NotNullWhen(true)] string phoneNumber)
    {
        if (!IsValid(phoneNumber, out var parsedNumber))
            return null;

        return _phoneUtil.Format(parsedNumber, PhoneNumberFormat.INTERNATIONAL);
    }

    /// <summary>
    /// Formats Turkish phone number to national format (0555 123 45 67)
    /// </summary>
    public static string? FormatTurkishNational([NotNullWhen(true)] string phoneNumber)
    {
        if (!IsValidTurkishNumber(phoneNumber))
            return null;

        var normalized = new string(phoneNumber.Where(char.IsDigit).ToArray());

        if (IsValidWithRegion(normalized, "TR", out var parsedNumber))
        {
            return _phoneUtil.Format(parsedNumber, PhoneNumberFormat.NATIONAL);
        }

        return null;
    }

    /// <summary>
    /// Gets the region code for a phone number
    /// </summary>
    public static string? GetRegionCode([NotNullWhen(true)] string phoneNumber)
    {
        if (!IsValid(phoneNumber, out var parsedNumber))
            return null;

        return _phoneUtil.GetRegionCodeForNumber(parsedNumber);
    }

    /// <summary>
    /// Gets the phone number type (MOBILE, FIXED_LINE, etc.)
    /// </summary>
    public static PhoneNumberType? GetNumberType([NotNullWhen(true)] string phoneNumber)
    {
        if (!IsValid(phoneNumber, out var parsedNumber))
            return null;

        return _phoneUtil.GetNumberType(parsedNumber);
    }
}