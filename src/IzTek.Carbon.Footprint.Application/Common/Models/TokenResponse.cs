namespace IzTek.Carbon.Footprint.Application.Common.Models;

public record TokenResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    string RefreshToken);