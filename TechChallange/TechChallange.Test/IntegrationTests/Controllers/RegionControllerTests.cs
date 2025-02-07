using System.Net;
using System.Text.Json;
using TechChallange.Api.Controllers.Region.Dto;
using TechChallange.Api.Response;

namespace TechChallange.Test.IntegrationTests.Controllers
{
    public class RegionControllerTests : IClassFixture<TechChallangeApplicationFactory>
    {
        private readonly TechChallangeApplicationFactory _techChallangeApplicationFactory;

        public RegionControllerTests(TechChallangeApplicationFactory techChallangeApplicationFactory)
        {
            _techChallangeApplicationFactory = techChallangeApplicationFactory;
        }

        [Fact]
        public async Task Test()
        {
            try
            {
                System.Console.WriteLine("Teste 1");
                var client = _techChallangeApplicationFactory.CreateClient();

               //// var response = await client.GetAsync("region/get-all?pageSize=10&page=1");

               // var resp = await response.Content.ReadAsStringAsync();

               // var result = JsonSerializer.Deserialize<BaseResponsePagedDto<IEnumerable<RegionResponseDto>>>(resp,
               //         new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

               // Assert.Equal(HttpStatusCode.OK, response.StatusCode);
               // Assert.NotNull(result?.Data);

                
            }
            catch (Exception ex)
            {
                
                throw new Exception("Erro aqui.");
            }
         
        }

        //[Fact]
        //public async Task Teste2()
        //{
        //    var client = _techChallangeApplicationFactory.CreateClient();


        //    var response = await client.GetAsync("region/get-all?pageSize=10&page=1");

        //    var resp = await response.Content.ReadAsStringAsync();

        //    var result = JsonSerializer.Deserialize<BaseResponsePagedDto<IEnumerable<RegionResponseDto>>>(resp,
        //            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


        //    var responseid = await client.GetAsync($"region/get-by-id/{result?.Data?.FirstOrDefault()?.Id}");

        //    var respId = await responseid.Content.ReadAsStringAsync();

        //    var resultId = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(respId,
        //    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    Assert.Equal(HttpStatusCode.OK, responseid.StatusCode);
        //    Assert.Equal(result?.Data?.FirstOrDefault(), resultId?.Data);
        //}
    }
}
