using AutoMapper;
using VShop.Products.Dto;
using VShop.Products.Models;

namespace VShop.Products.Mappings
{
    public class MappingProfile : Profile   
    {

        MapperConfiguration configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Category, CategoryDto>()
               .ReverseMap();
        });

    }
}
