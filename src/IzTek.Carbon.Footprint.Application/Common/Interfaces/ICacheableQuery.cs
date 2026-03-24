namespace IzTek.Carbon.Footprint.Application.Common.Interfaces;

// Cache'lenmesini istediğin Query'ler bu interface'i implemente eder
public interface ICacheableQuery
{
    string CacheKey { get; } 
    TimeSpan? Expiry => TimeSpan.FromMinutes(30);
}

