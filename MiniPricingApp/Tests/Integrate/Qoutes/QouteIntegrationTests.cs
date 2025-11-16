using FluentAssertions;
using MiniPricingApp.Modules.Qoutes.Application.Dtos;
using MiniPricingApp.Shares.Common;
using MiniPricingApp.Tests.Integrate.Factory;
using Xunit;

namespace MiniPricingApp.Tests.Integrate.Qoutes
{
    public class QouteIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public QouteIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CalculatePrice_ReturnsPrice()
        {
            var request = new PricingCalculationRequestDto
            {
                weight = 10,
                areaCode = "1000"
            };

            var response = await _client.PostAsJsonAsync("/qoutes/price", request);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<BaseResponse<PricingCalculationResponseDto>>();

            result.Should().NotBeNull();
            result?.isSuccess.Should().BeTrue();
            result?.data.finalPrice.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task UploadCsvFile_ReturnsJobId()
        {
            // Create a temporary CSV file
            var tempCsvPath = Path.Combine(Path.GetTempPath(), "test.csv");
            await File.WriteAllTextAsync(tempCsvPath, "Weight,AreaCode\n10,NNN");

            using var stream = File.OpenRead(tempCsvPath);
            var formData = new MultipartFormDataContent();
            formData.Add(new StreamContent(stream), "file", "test.csv");

            var response = await _client.PostAsync("/qoutes/bulk", formData);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<BaseResponse<CreateAndUpdateResponse>>();
            result?.isSuccess.Should().BeTrue();
        }
    }
}
