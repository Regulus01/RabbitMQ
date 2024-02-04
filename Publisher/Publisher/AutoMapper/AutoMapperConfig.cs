using AutoMapper;
using Publisher.Domain;
using Publisher.ViewModels;

namespace Publisher.AutoMapper;

public class AutoMapperConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        return new MapperConfiguration(config =>
        {
            config.CreateMap<Orders, GetOrderViewModel>();
            config.CreateMap<Products, GetProductOrderViewModel>();
            config.CreateMap<Products, GetProductsViewModel>();
        });
    }
}