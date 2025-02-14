using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using System.Text.Json;
using TechChallange.Api.Controllers.Contact.Dto;
using TechChallange.Api.Controllers.Region.Dto;
using TechChallange.Api.Response;
using TechChallange.Test.IntegrationTests.Setup;

namespace TechChallange.Test.IntegrationTests.Controllers
{
    public class ContactControllerTests(TechChallangeApplicationFactory techChallangeApplicationFactory) : BaseIntegrationTest(techChallangeApplicationFactory)
    {
        const string defaultMessageExceptionRegion = "Região não encontrada na base dados.";
        const string defaultMessageExceptionContact = "Contato não encontrado na base de dados.";

        [Fact(DisplayName = "Should Create New Contact With Success")]
        public async Task ShouldCreateNewContactWithSuccess()
        {

            var client = techChallangeApplicationFactory.CreateClient();
            var regionEntity = await _dbContext.Region.FirstOrDefaultAsync(r => r.Ddd == "11");

            var contact = new ContactCreateDto
            {
                Name = "Teste",
                RegionId = regionEntity.Id,
                Email = "newcontact@teste.com",
                Phone = "36364141"
            };

            var content = new StringContent(JsonSerializer.Serialize(contact), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("contact", content);

            var responseParsed = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<IEnumerable<RegionResponseDto>>>(responseParsed,
                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var contactDb = await _dbContext.Contact.AsNoTracking().FirstOrDefaultAsync(r => r.Phone == contact.Phone);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal(contactDb.Name, contact.Name);
            Assert.Equal(contactDb.Email, contact.Email);
            Assert.Equal(contactDb.Phone, contact.Phone);
        }


        [Fact(DisplayName = "Should Create Contact Return Bad Request When Region Does Not Exist")]
        public async Task ShouldCreateContactReturnBadRequestWhenRegionDoesNotExist()
        {
            var client = techChallangeApplicationFactory.CreateClient();

            var contact = new ContactCreateDto
            {
                Name = "Teste",
                RegionId = Guid.NewGuid(),
                Email = "newcontact@teste.com",
                Phone = "36364141"
            };

            var content = new StringContent(JsonSerializer.Serialize(contact), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("contact", content);

            var responseParsed = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<IEnumerable<RegionResponseDto>>>(responseParsed,
                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var contactDb = await _dbContext.Contact.AsNoTracking().FirstOrDefaultAsync(r => r.Phone == contact.Phone);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(result.Success);
            Assert.Equal(defaultMessageExceptionRegion, result.Error);
        }

        [Theory(DisplayName = "Should Return Contacts By Ddd")]
        [InlineData("11")]
        [InlineData("47")]
        public async Task ShouldReturnContactsByDdd(string ddd)
        {
            
            var client = techChallangeApplicationFactory.CreateClient();
            var regionDb = await _dbContext.Region.AsNoTracking().FirstOrDefaultAsync(r => r.Ddd == ddd);

            var response = await client.GetAsync($"contact/by-ddd/{ddd}");

            var responseParsed = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<IEnumerable<ContactResponseDto>>>(responseParsed,
                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(result.Data);
            Assert.Equal(regionDb.Id, result.Data.FirstOrDefault().RegionId);

        }

        [Theory(DisplayName = "Should Return Contact By Id")]
        [InlineData("11")]
        [InlineData("47")]
        public async Task ShouldReturnContactById(string ddd)
        {

            var client = techChallangeApplicationFactory.CreateClient();
            var regionDb = await _dbContext.Region.AsNoTracking().FirstOrDefaultAsync(r => r.Ddd == ddd);

            var contactDb = await _dbContext.Contact.AsNoTracking().FirstOrDefaultAsync(r => r.RegionId == regionDb.Id);

            var response = await client.GetAsync($"contact/by-id/{contactDb.Id}");

            var responseParsed = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize< BaseResponseDto<ContactResponseDto>>(responseParsed,
                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal(contactDb.Id, result.Data.Id);

        }

        [Fact(DisplayName = "Should Get By Id Return Bad Request When It Does Not Exist")]
        public async Task ShouldGetByIdReturnBadRequestWhenItDoesNotExist()
        {

            var client = techChallangeApplicationFactory.CreateClient();

            var response = await client.GetAsync($"contact/by-id/{Guid.NewGuid()}");

            var responseParsed = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<ContactResponseDto>>(responseParsed,
                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Null(result.Data);
            Assert.False(result.Success);
            Assert.Equal(defaultMessageExceptionContact, result.Error);

        }
    }
}
