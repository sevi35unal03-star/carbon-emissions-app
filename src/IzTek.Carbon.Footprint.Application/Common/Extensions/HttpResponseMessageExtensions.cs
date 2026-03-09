namespace IzTek.Carbon.Footprint.Application.Common.Extensions;

public static class HttpResponseMessageExtensions
{
    private static readonly JsonSerializerOptions _defaultOptions = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static async Task<T?> DeserializeAsync<T>(this HttpResponseMessage response, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        return await JsonSerializer.DeserializeAsync<T>(stream, options ?? _defaultOptions, cancellationToken);
    }
}