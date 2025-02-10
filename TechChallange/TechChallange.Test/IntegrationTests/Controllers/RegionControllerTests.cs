using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using TechChallange.Api.Controllers.Region.Dto;
using TechChallange.Api.Response;
using TechChallange.Domain.Region.Entity;

namespace TechChallange.Test.IntegrationTests.Controllers
{
    public class RegionControllerTests(TechChallangeApplicationFactory techChallangeApplicationFactory) : BaseIntegrationTest(techChallangeApplicationFactory)
    {

     

        [Fact]
        public async Task ShouldDeleteLogicalRegionById()
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

            //var regionDb = await _dbContext.Region.FirstOrDefaultAsync(r => r.Id == regionEntity.Id);


            //var responseid = await client.GetAsync($"region/get-by-id/{regionEntity.Id}");

            //var respId = await responseid.Content.ReadAsStringAsync();

            //var resultId = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(respId,
            //new JsonSerializerOptions { PropertyNameCaseInsensitive = true });



            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.True(regionDb.IsDeleted);
        }
    }
}
