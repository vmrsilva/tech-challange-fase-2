using AutoMapper;
using TechChallange.Api.Controllers.Contact.Dto;
using TechChallange.Api.Controllers.Region.Dto;
using TechChallange.Domain.Contact.Entity;
using TechChallange.Domain.Region.Entity;

namespace TechChallange.Api.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegionCreateDto, RegionEntity>();
            CreateMap<RegionEntity, RegionResponseDto>();
            CreateMap<RegionEntity, RegionWithContactsResponseDto>();
            CreateMap<RegionUpdateDto, RegionEntity>();

            CreateMap<ContactCreateDto, ContactEntity>();
            CreateMap<ContactUpdateDto, ContactEntity>();
            CreateMap<ContactEntity, ContactResponseDto>();
        }
    }
}
