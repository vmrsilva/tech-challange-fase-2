using Azure.Core;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Text.Json;
using TechChallange.Api.Controllers.Region.Dto;
using TechChallange.Api.Response;
using TechChallange.Domain.Contact.Entity;
using TechChallange.Domain.Region.Entity;
using TechChallange.Domain.Region.Exception;
using TechChallange.Test.IntegrationTests.Setup;

namespace TechChallange.Test.IntegrationTests.Controllers
{
    public class RegionControllerTests(TechChallangeApplicationFactory techChallangeApplicationFactory) : BaseIntegrationTest(techChallangeApplicationFactory)
    {

        const string defaultMessageException = "Região não encontrada na base dados.";

        [Fact(DisplayName = "Should Return All Activies Regions Paged")]
        public async Task ShouldReturnAllActiviesRegionsPaged()
        {
            var client = techChallangeApplicationFactory.CreateClient();

            var countInDb = await _dbContext.Region.AsNoTracking().CountAsync(c => c.IsDeleted == false);

            var response = await client.GetAsync("region/get-all?pageSize=10&page=1");

            var responseParsed = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponsePagedDto<IEnumerable<RegionResponseDto>>>(responseParsed,
                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result?.Data);
            Assert.Equal(countInDb, result?.Data.Count());
        }

        [Fact(DisplayName = "Should Return Region By Id")]
        public async Task ShouldReturnRegionById()
        {
            var regionEntity = await _dbContext.Region.FirstOrDefaultAsync(r => r.Ddd == "11");

            var client = techChallangeApplicationFactory.CreateClient();

            var response = await client.GetAsync($"region/get-by-id/{regionEntity.Id}");

            var responseParsed = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(responseParsed,
                                                                                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(regionEntity.Id, result?.Data.Id);
            Assert.True(result?.Success);
            Assert.Equal(regionEntity.Ddd, result?.Data.Ddd);
        }

        [Fact(DisplayName = "Should Get Region By Id Return Bad Request When Id Does Not Exist")]
        public async Task ShouldGetRegionByIdReturnBadRequestWhenIdDoesNotExist()
        {
            var client = techChallangeApplicationFactory.CreateClient();

            var response = await client.GetAsync($"region/get-by-id/{Guid.NewGuid()}");

            var responseContent = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(responseContent,
                                                                                          new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(result?.Success);
            Assert.Equal(defaultMessageException, result?.Error);
            Assert.Null(result?.Data);
        }

        [Fact(DisplayName = "Should Delete Logically Region By Id With Success")]
        public async Task ShouldDeleteLogicallyRegionByIdWithSuccess()
        {
            var regionEntity = await _dbContext.Region.FirstOrDefaultAsync(r => r.Ddd == "11");

            var client = techChallangeApplicationFactory.CreateClient();

            var response = await client.DeleteAsync($"region/{regionEntity.Id}");

            var responseParsed = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(responseParsed,
                                                                                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _dbContext.Entry(regionEntity).State = EntityState.Detached;
            var regionDb = await _dbContext.Region.AsNoTracking().FirstOrDefaultAsync(r => r.Id == regionEntity.Id);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.True(regionDb?.IsDeleted);
        }
        [Fact(DisplayName = "Should Delete Region By Id Return Bad Request When It Does Not Exist")]
        public async Task ShouldDeleteRegionByIdReturnBadRequestWhenItDoesNotExist()
        {
            var regionEntity = await _dbContext.Region.FirstOrDefaultAsync(r => r.Ddd == "11");

            var client = techChallangeApplicationFactory.CreateClient();

            var response = await client.DeleteAsync($"region/{Guid.NewGuid()}");

            var responseParsed = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(responseParsed,
                                                                                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _dbContext.Entry(regionEntity).State = EntityState.Detached;
            var regionDb = await _dbContext.Region.AsNoTracking().FirstOrDefaultAsync(r => r.Id == regionEntity.Id);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(regionDb?.IsDeleted);
            Assert.Equal(defaultMessageException, result?.Error);
        }

        [Fact(DisplayName = "Should Update Region With Success")]
        public async Task ShouldUpdateRegionWithSuccess()
        {
            var regionEntity = await _dbContext.Region.FirstOrDefaultAsync(r => r.Ddd == "47");

            var client = techChallangeApplicationFactory.CreateClient();

            var regionUpdateDto = new RegionUpdateDto
            {
                Id = regionEntity.Id,
                Name = "Update Test",
                Ddd = "95"
            };

            var json = JsonSerializer.Serialize(regionUpdateDto);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync("region", data);

            var responseContent = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(responseContent,
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

        [Fact(DisplayName = "Should Update Region Return Bad Request When It Does Not Exist")]
        public async Task ShouldUpdateRegionReturnBadRequestWhenItDoesNotExist()
        {
            var regionEntity = await _dbContext.Region.FirstOrDefaultAsync(r => r.Ddd == "11");

            var client = techChallangeApplicationFactory.CreateClient();

            var regionUpdateDto = new RegionUpdateDto
            {
                Id = Guid.NewGuid(),
                Name = "São Paulo",
                Ddd = "95"
            };

            var json = JsonSerializer.Serialize(regionUpdateDto);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync("region", data);

            var responseContent = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(responseContent,
                                                                                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _dbContext.Entry(regionEntity).State = EntityState.Detached;
            var regionDb = await _dbContext.Region.AsNoTracking().FirstOrDefaultAsync(r => r.Id == regionEntity.Id);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(result?.Success);
            Assert.Equal(defaultMessageException, result?.Error);
        }

        [Fact(DisplayName = "Should Return Region By Ddd With Contacts")]
        public async Task ShouldReturnRegionByDddWithContacts()
        {
            var client = techChallangeApplicationFactory.CreateClient();

            var response = await client.GetAsync($"region/get-ddd-with-contacts/{11}");
            var responseParsed = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<BaseResponseDto<RegionWithContactsResponseDto>>(responseParsed,
                                                                                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result?.Success);
            Assert.NotNull(result?.Data);
            Assert.NotNull(result?.Data.Name);
            Assert.NotNull(result?.Data.Contacts);
        }

        [Fact(DisplayName = "Should Get Region By Ddd With Contacts Return Data Equal Null When Ddd Does Not Exist")]
        public async Task ShouldGetRegionByDddWithContactsReturnBadRequestWhenDddDoesNotExist()
        {
            var client = techChallangeApplicationFactory.CreateClient();

            var response = await client.GetAsync($"region/get-ddd-with-contacts/{99}");
            var responseParsed = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<BaseResponseDto<RegionWithContactsResponseDto>>(responseParsed,
                                                                                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result?.Success);
            Assert.Null(result?.Data);
        }

        [Fact(DisplayName = "Should Return Region By Ddd ")]
        public async Task ShouldReturnRegionByDdd()
        {
            var client = techChallangeApplicationFactory.CreateClient();

            var response = await client.GetAsync($"region/get-by-ddd/{11}");
            var responseParsed = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(responseParsed,
                                                                                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result?.Success);
            Assert.NotNull(result?.Data);
            Assert.NotNull(result?.Data.Name);
        }

        [Fact(DisplayName = "Should Get Region By Ddd Return Data Equal Null When Ddd Does Not Exist")]
        public async Task ShouldGetRegionByDddReturnDataEqualNullWhenDddDoesNotExist()
        {

            var client = techChallangeApplicationFactory.CreateClient();

            var response = await client.GetAsync($"region/get-by-ddd/{99}");
            var responseParsed = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(responseParsed,
                                                                                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result?.Success);
            Assert.Null(result?.Data);
        }
    }
}
