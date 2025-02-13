using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TechChallange.Api.Controllers.Contact.Dto;
using TechChallange.Api.Controllers.Region.Dto;
using TechChallange.Api.Response;
using TechChallange.Test.IntegrationTests.Setup;

namespace TechChallange.Test.IntegrationTests.Controllers
{
    public class ContactControllerTests(TechChallangeApplicationFactory techChallangeApplicationFactory) : BaseIntegrationTest(techChallangeApplicationFactory)
    {
        const string defaultMessageException = "Região não encontrada na base dados.";


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
    }
}
