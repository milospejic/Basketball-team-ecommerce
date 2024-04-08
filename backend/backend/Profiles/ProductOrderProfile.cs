using AutoMapper;
using backend.Models.Dtos;
using backend.Models;

namespace backend.Profiles
{
    public class ProductOrderProfile : Profile
    {
        public ProductOrderProfile()
        {
            CreateMap<ProductOrder, ProductOrderDto>();
            CreateMap<ProductOrderDto, ProductOrder>();
            CreateMap<ProductOrderCreateDto, ProductOrder>();
            //CreateMap<ProductOrderUpdateDto, ProductOrder>();
        }
    }

}
