using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using UnitTests.ApplicationCore.Specifications;
using UnitTests.Builders;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Repositories
{
    public class CurrencyRateRepositoryTests : IDisposable
    {
        readonly CurrencyRateContext currencyRateContext;
        readonly CurrencyRateRepository currencyRateRepository;
        readonly ITestOutputHelper output;
        readonly ITimeService timeService;

        public CurrencyRateRepositoryTests(ITestOutputHelper output)
        {
            this.output = output;
            var dbOptions = new DbContextOptionsBuilder<CurrencyRateContext>()
                .UseInMemoryDatabase(databaseName: "TestCurrencyRates")
                .Options;
            currencyRateContext = new CurrencyRateContext(dbOptions);
            timeService = new ManagedTimeService();
            currencyRateRepository = new CurrencyRateRepository(currencyRateContext, timeService);
        }

        [Fact]
        public void RepoThreeBTCsNonCanonicalCase()
        {
            // Arrange
            var spec = new RateOfCurrencySpecification("btc");
            currencyRateContext.CurrencyRates.AddRange(RatesOfCurrency.GetTestRates(setIds: false));
            currencyRateContext.SaveChanges();

            // Act
            var result = currencyRateRepository.List(spec);

            Assert.Collection(result,
                item => Assert.Equal(Currency.BTC, item.Currency),
                item => Assert.Equal(Currency.BTC, item.Currency),
                item => Assert.Equal(Currency.BTC, item.Currency)
            );
        }

        [Fact]
        public void RepoEthTooOld()
        {
            // Arrange
            var all = RatesOfCurrency.GetTestRates(setIds: false, now: timeService.Now);
            var ethRate = all[3];
            ethRate.DateTime = timeService.Now.AddDays(-1);
            currencyRateContext.CurrencyRates.AddRange(all);
            currencyRateContext.SaveChanges();

            // Act
            var averageRate = currencyRateRepository.GetAverageRateBy24h("eth");

            // Assert
            Assert.Null(averageRate);
        }

        [Fact]
        public async void RepoEthTooOldAsync()
        {
            // Arrange
            var all = RatesOfCurrency.GetTestRates(setIds: false, now: timeService.Now);
            var ethRate = all[3];
            ethRate.DateTime = timeService.Now.AddDays(-1);
            currencyRateContext.CurrencyRates.AddRange(all);
            await currencyRateContext.SaveChangesAsync();

            // Act
            var averageRate = await currencyRateRepository.GetAverageRateBy24hAsync("eth");

            // Assert
            Assert.Null(averageRate);
        }

        [Fact]
        public void RepoEthNotTooOld()
        {
            // Arrange
            var all = RatesOfCurrency.GetTestRates(setIds: false, now: timeService.Now);
            var ethRate = all[3];
            ethRate.DateTime = timeService.Now.AddDays(-1).AddMilliseconds(1);
            currencyRateContext.CurrencyRates.AddRange(all);
            currencyRateContext.SaveChanges();

            // Act
            var averageRage = currencyRateRepository.GetAverageRateBy24h("eth");

            // Assert
            Assert.NotNull(averageRage);
            Assert.Equal(ethRate.Rate, averageRage.Value);
        }

        [Fact]
        public async void RepoEthNotTooOldAsync()
        {
            // Arrange
            var all = RatesOfCurrency.GetTestRates(setIds: false, now: timeService.Now);
            var ethRate = all[3];
            ethRate.DateTime = timeService.Now.AddDays(-1).AddMilliseconds(1);
            currencyRateContext.CurrencyRates.AddRange(all);
            await currencyRateContext.SaveChangesAsync();

            // Act
            var averageRate = await currencyRateRepository.GetAverageRateBy24hAsync("eth");

            // Assert
            Assert.NotNull(averageRate);
            Assert.Equal(ethRate.Rate, averageRate.Value);
        }

        [Fact]
        public void RepoThreeBtcAverage()
        {
            // Arrange
            var all = RatesOfCurrency.GetTestRates(setIds: false, now: timeService.Now);
            var expectedAvg = (all[0].Rate + all[1].Rate + all[2].Rate) / 3m;
            currencyRateContext.CurrencyRates.AddRange(all);
            currencyRateContext.SaveChanges();

            // Act
            var averageRage = currencyRateRepository.GetAverageRateBy24h("btc");

            // Assert
            Assert.NotNull(averageRage);
            Assert.Equal(expectedAvg, averageRage.Value);
        }

        public void Dispose()
        {
            currencyRateContext.CurrencyRates.RemoveRange(currencyRateContext.CurrencyRates);
            currencyRateContext.SaveChanges();
        }
    }
}
