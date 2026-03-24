namespace IzTek.Carbon.Footprint.Application.Common.Interfaces;

public interface ICacheInvalidator
{
    IEnumerable<string> CacheKeys { get; }
}