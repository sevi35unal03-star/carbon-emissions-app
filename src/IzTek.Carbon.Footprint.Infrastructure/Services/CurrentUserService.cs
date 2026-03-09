namespace IzTek.Carbon.Footprint.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId
        => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? UserName
        => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

    public bool IsAuthenticated
        => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}