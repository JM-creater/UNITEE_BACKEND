using AutoMapper;
using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;

namespace UNITEE_BACKEND.AutoMapperConfig
{
    public class AutoMapperConfigProfile : Profile
    {
        public AutoMapperConfigProfile()
        {
            // SizeQuantity
            CreateMap<Product, ProductWithSizeQuantityDto>();
            CreateMap<UpdateSizeQuantityDto, SizeQuantity>();
                //.ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size));
            CreateMap<CreateSizeQuantityDto, SizeQuantity>();
            CreateMap<SizeQuantityDto, SizeQuantity>();
            CreateMap<SizeQuantity, GetSizeQuantityByIdDto>();

            // Product
            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());
        }
    }
}
