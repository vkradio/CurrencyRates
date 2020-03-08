using ApplicationCore.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTests.ApplicationCore.Specifications;
using Xunit;

namespace FunctionalTests.Web.Controllers
{
    public class ApiCurrencyRateController : BaseWebTest
    {
        const string c_baseUri = "/api/currency-rates";

        [Fact]
        public async Task Returns3Btcs()
        {
            // Arrange
            var url = c_baseUri + "/btc";

            // Act
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<List<CurrencyRate>>(stringResponse);

            // Assert
            Assert.Equal(3, model.Count);
        }

        [Fact]
        public async Task Returns1Eth()
        {
            // Arrange
            var url = c_baseUri + "/ETH";

            // Act
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<List<CurrencyRate>>(stringResponse);

            // Assert
            Assert.Collection(model,
                item => Assert.Equal(item.Currency, Currency.ETH)
            );
        }

        [Fact]
        public async Task ReturnsEmptyForUnknownCurrency()
        {
            // Arrange
            var url = c_baseUri + "/unk";

            // Act
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<List<CurrencyRate>>(stringResponse);

            // Assert
            Assert.Empty(model);
        }

        [Fact]
        public async Task ReturnsAverageBtc()
        {
            // Arrange
            var expectedBtcs = RatesOfCurrency
                .GetTestRates(now: timeService.Now)
                .Where(c => c.Currency == Currency.BTC)
                .ToList();
            var expectedAverage = (expectedBtcs[0].Rate + expectedBtcs[1].Rate + expectedBtcs[2].Rate) / 3m;
            var url = c_baseUri + "/btc/avg24h";

            // Act
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var average = JsonConvert.DeserializeObject<decimal>(stringResponse);

            // Assert
            Assert.Equal(expectedAverage, average);
        }

        [Fact]
        public async Task ReturnsNotFoundForAverageEth()
        {
            var response = await httpClient.GetAsync(c_baseUri + "/eth/avg24h");
            Assert.Equal(404, (int)response.StatusCode);
            //Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());
        }
    }
}
