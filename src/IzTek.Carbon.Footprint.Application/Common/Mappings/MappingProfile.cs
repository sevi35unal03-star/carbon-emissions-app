namespace IzTek.Carbon.Footprint.Application.Common.Mappings;

public class MappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, GetAllProductsResponse>().TwoWays();
    }
}