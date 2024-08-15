using AutoMapper;
using VShop.Products.Dto;
using VShop.Products.Models;

namespace VShop.Products.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            CreateMap<Category, CategoryDto>().ReverseMap();

            CreateMap<ProductDto, Product>();
            CreateMap<Product, ProductDto>()
                .ForMember(dto => dto.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        }
    }
}