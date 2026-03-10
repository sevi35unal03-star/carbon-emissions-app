namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.DonateTrees;

public record DonateTreesCommand(Guid UserId) : ICacheInvalidator
{
    public IEnumerable<string> CacheKeys =>
        [$"donation-history:{UserId}"];
}