using ApplicationCore.Entities;
using ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UnitTests.ApplicationCore.Specifications
{
    public class RatesOfCurrency
    {
        [Fact]
        public void UnknownCurrencyCode()
        {
            // Arrange
            var spec = new RateOfCurrencySpecification("UNK");

            // Act
            var hasAny = GetTestRates()
                .AsQueryable()
                .Any(spec.Criteria);

            // Assert
            Assert.False(hasAny);
        }

        [Fact]
        public void NullCurrencyCode()
        {
            // Arrange
            var spec = new RateOfCurrencySpecification(null);

            // Act
            var hasAny = GetTestRates()
                .AsQueryable()
                .Any(spec.Criteria);

            // Assert
            Assert.False(hasAny);
        }

        [Fact]
        public void ThreeBTCsNonCanonicalCase()
        {
            // Arrange
            var spec = new RateOfCurrencySpecification("btc");

            // Act
            var result = GetTestRates()
                .AsQueryable()
                .Where(spec.Criteria);

            // Assert
            Assert.Collection(result,
                item => Assert.Equal(Currency.BTC, item.Currency),
                item => Assert.Equal(Currency.BTC, item.Currency),
                item => Assert.Equal(Currency.BTC, item.Currency)
            );
        }

        [Fact]
        public void EthTooOld()
        {
            // Arrange
            var all = GetTestRates();
            var spec = new RateOfCurrencySpecification(Currency.ETH, all[3].DateTime);

            // Act
            var result = all
                .AsQueryable()
                .Where(spec.Criteria);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void EthNotTooOld()
        {
            // Arrange
            var all = GetTestRates();
            var ethRate = all[3];
            var spec = new RateOfCurrencySpecification(Currency.ETH, ethRate.DateTime.AddMilliseconds(-1));

            // Act
            var result = all
                .AsQueryable()
                .Where(spec.Criteria);

            // Assert
            Assert.Collection(result,
                item => Assert.Equal(ethRate.Id, item.Id)
            );
        }

        public static List<CurrencyRate> GetTestRates(bool setIds = true, DateTime? now = null)
        {
            if (!now.HasValue)
                now = DateTime.UtcNow;
            return new List<CurrencyRate>
            {
                new CurrencyRate { Id = setIds ? 1 : 0, DateTime = now.Value.AddSeconds(-1), Currency = Currency.BTC, Rate = 1m },
                new CurrencyRate { Id = setIds ? 2 : 0, DateTime = now.Value.AddSeconds(-2), Currency = Currency.BTC, Rate = 1.01m },
                new CurrencyRate { Id = setIds ? 3 : 0, DateTime = now.Value.AddSeconds(-3), Currency = Currency.BTC, Rate = 1.03m },
                new CurrencyRate { Id = setIds ? 4 : 0, DateTime = now.Value.AddDays(-1), Currency = Currency.ETH, Rate = 4.03m }
            };
        }
    }
}
