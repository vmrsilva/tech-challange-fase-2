using System.Net;
using System.Text.Json;
using TechChallange.Api.Controllers.Region.Dto;
using TechChallange.Api.Response;
using TechChallange.Domain.Region.Entity;

namespace TechChallange.Test.IntegrationTests.Controllers
{
    public class RegionControllerTests (TechChallangeApplicationFactory techChallangeApplicationFactory): BaseIntegrationTest(techChallangeApplicationFactory)
    {

        [Fact]
        public async Task Test()
        {

                var client =techChallangeApplicationFactory.CreateClient();
                await _dbContext.Region.AddAsync(new RegionEntity("SP", "11"));
                await _dbContext.SaveChangesAsync();           


               var response = await client.GetAsync("region/get-all?pageSize=10&page=1");

                var resp = await response.Content.ReadAsStringAsync();

               var result = JsonSerializer.Deserialize<BaseResponsePagedDto<IEnumerable<RegionResponseDto>>>(resp,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var resulxxt = await _regionService.GetAllPagedAsync(10, 1);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
               Assert.NotNull(result?.Data);
               Assert.Equal(1,result?.Data.Count());

                Assert.NotNull(resulxxt);

        }

        [Fact]
        public async Task Teste2()
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
    }
}
