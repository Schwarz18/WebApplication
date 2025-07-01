using AutoMapper;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductReadDto>();
            //CreateMap<ProductReadDto, Product>();

            CreateMap<ProductDto, Product>();
        }
    }
}
