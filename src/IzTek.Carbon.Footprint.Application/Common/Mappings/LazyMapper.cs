namespace IzTek.Carbon.Footprint.Application.Common.Mappings;

public static class LazyMapper
{
    private static readonly Lazy<IMapper> _lazy = new(() =>
    {
        var config = new TypeAdapterConfig();

        config.Scan(Assembly.GetExecutingAssembly());

        config.Compile();

        return new Mapper(config);
    });

    public static IMapper Mapper => _lazy.Value;
}