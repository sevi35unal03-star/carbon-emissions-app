namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.DonateTrees;

public record DonateTreesCommand() : ICacheInvalidator
{
    public IEnumerable<string> CacheKeys =>
        [$"donation-history:"];
}