using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using System.Text.Json;
using TechChallange.Api.Controllers.Region.Dto;
using TechChallange.Api.Response;
using TechChallange.Domain.Region.Entity;

namespace TechChallange.Test.IntegrationTests.Controllers
{
    public class RegionControllerTests(TechChallangeApplicationFactory techChallangeApplicationFactory) : BaseIntegrationTest(techChallangeApplicationFactory)
    {

        [Fact(DisplayName = "Should Return All Activy Regions Paged")]
        public async Task ShouldReturnAllActiviesRegionsPaged()
        {

            var client = techChallangeApplicationFactory.CreateClient();
            await _dbContext.Region.AddAsync(new RegionEntity("SP", "11"));
            await _dbContext.SaveChangesAsync();

            var count = await _dbContext.Region.CountAsync(c => c.IsDeleted == false);


            var response = await client.GetAsync("region/get-all?pageSize=10&page=1");

            var resp = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponsePagedDto<IEnumerable<RegionResponseDto>>>(resp,
                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result?.Data);
            Assert.Equal(count, result?.Data.Count());
        }

        [Fact(DisplayName = "Should Return Region By Id")]
        public async Task ShouldReturnRegionById()
        {
            var regionEntity = new RegionEntity("SP", "11");
            await _dbContext.Region.AddAsync(regionEntity);
            await _dbContext.SaveChangesAsync();

            var client = techChallangeApplicationFactory.CreateClient();

            var responseid = await client.GetAsync($"region/get-by-id/{regionEntity.Id}");

            var respId = await responseid.Content.ReadAsStringAsync();

            var resultId = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(respId,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.OK, responseid.StatusCode);
            Assert.Equal(regionEntity.Id, resultId?.Data.Id);
        }

        [Fact(DisplayName = "Should Return Region By Id Return Bad Request When Id Does Not Exist")]
        public async Task ShouldReturnRegionByIdReturnBadRequestWhenIdDoesNotExist()
        {
            //var regionEntity = new RegionEntity("SP", "11");
            //await _dbContext.Region.AddAsync(regionEntity);
            //await _dbContext.SaveChangesAsync();

            var client = techChallangeApplicationFactory.CreateClient();

            var responseid = await client.GetAsync($"region/get-by-id/{Guid.NewGuid()}");

            var respId = await responseid.Content.ReadAsStringAsync();

            var resultId = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(respId,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.BadRequest, responseid.StatusCode);
            Assert.False(resultId.Success);
            Assert.Equal("Região não encontrada na base dados.", resultId.Error);
            Assert.Null(resultId.Data); 
        }

        [Fact(DisplayName = "Should Delete Logically Region By Id With Success")]
        public async Task ShouldDeleteLogicallyRegionByIdWithSuccess()
        {
            var regionEntity = new RegionEntity("SP", "11");
            await _dbContext.Region.AddAsync(regionEntity);
            await _dbContext.SaveChangesAsync();

            var client = techChallangeApplicationFactory.CreateClient();

            var response = await client.DeleteAsync($"region/{regionEntity.Id}");

            var resp = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(resp,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _dbContext.Entry(regionEntity).State = EntityState.Detached;
            var regionDb = await _dbContext.Region.AsNoTracking().FirstOrDefaultAsync(r => r.Id == regionEntity.Id);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.True(regionDb.IsDeleted);
        }

        [Fact(DisplayName = "Should Update Region With Success")]
        public async Task ShouldUpdateRegionWithSuccess()
        {
            var regionEntity = new RegionEntity("SP", "11");
            await _dbContext.Region.AddAsync(regionEntity);
            await _dbContext.SaveChangesAsync();

            var client = techChallangeApplicationFactory.CreateClient();

            var regionUpdateDto = new RegionUpdateDto
            {
                Id = regionEntity.Id,
                Name = "São Paulo",
                Ddd = "95"
            };

            var json = JsonSerializer.Serialize(regionUpdateDto);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync("region", data);

            var resp = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(resp,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _dbContext.Entry(regionEntity).State = EntityState.Detached;
            var regionDb = await _dbContext.Region.AsNoTracking().FirstOrDefaultAsync(r => r.Id == regionEntity.Id);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal(regionEntity.Id, regionDb?.Id);
            Assert.Equal(regionUpdateDto.Name, regionDb?.Name);
            Assert.NotEqual(regionEntity.Name, regionDb?.Name);
            Assert.Equal(regionUpdateDto.Ddd, regionDb?.Ddd);
            Assert.NotEqual(regionEntity.Ddd, regionDb?.Ddd);
        }
    }
}
