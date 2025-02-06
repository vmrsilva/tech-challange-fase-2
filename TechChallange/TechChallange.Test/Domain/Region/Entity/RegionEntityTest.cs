using AutoFixture;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechChallange.Api.Controllers.Contact.Dto;
using TechChallange.Api.Controllers.Region.Dto;
using TechChallange.Domain.Region.Entity;

namespace TechChallange.Test.Domain.Region.Entity
{
    public class RegionEntityTest
    {
        private readonly IMapper _mapper;
        public RegionEntityTest()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegionCreateDto, RegionEntity>();
            });

            _mapper = config.CreateMapper();
        }

        [Fact(DisplayName = "Should Create Entity Region With Exactly Props")]
        public void ShouldCreateEntityRegionWithExactlyProps()
        {
            var regionDto = new Fixture().Create<RegionCreateDto>();

            var entity = _mapper.Map<RegionEntity>(regionDto);

            Assert.IsType<Guid>(entity.Id);
            Assert.Equal(regionDto.Name, entity.Name);
            Assert.Equal(regionDto.Ddd, entity.Ddd);
            Assert.False(entity.IsDeleted);
        }
    }
}
