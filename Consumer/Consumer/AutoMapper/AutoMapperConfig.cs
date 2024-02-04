using AutoMapper;
using Consumer.Domain;
using Consumer.ViewModels;

namespace Consumer.AutoMapper;

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