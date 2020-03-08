using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class CurrencyRateRepository : EfRepository<CurrencyRate>, ICurrencyRateRepository
    {
        readonly ITimeService timeService;

        public CurrencyRateRepository(CurrencyRateContext dbContext, ITimeService timeService) :
            base(dbContext) => this.timeService = timeService;

        public decimal? GetAverageRateBy24h(string currencyCode)
        {
            var limitTime = timeService.Now.AddDays(-1);

            var average = dbContext.CurrencyRates
                .Where(cr => cr.Currency == currencyCode.ToUpper())
                .Where(cr => cr.DateTime > limitTime)
                .Select(cr => cr.Rate)
                .DefaultIfEmpty(0m)
                .Average();

            // If we got 0, it maybe because there are no DB records. In that case we should
            // return null instead of 0.
            if (average == 0m)
            {
                var count = dbContext.CurrencyRates
                    .Where(cr => cr.Currency == currencyCode.ToUpper())
                    .Where(cr => cr.DateTime > limitTime)
                    .Count();
                if (count == 0)
                    return null;
            }

            return average;
        }

        public async Task<decimal?> GetAverageRateBy24hAsync(string currencyCode)
        {
            var limitTime = timeService.Now.AddDays(-1);

            var average = await dbContext.CurrencyRates
                .Where(cr => cr.Currency == currencyCode.ToUpper())
                .Where(cr => cr.DateTime > limitTime)
                .Select(cr => cr.Rate)
                .DefaultIfEmpty(0m)
                .AverageAsync();

            // If we got 0, it maybe because there are no DB records. In that case we should
            // return null instead of 0.
            if (average == 0m)
            {
                var count = await dbContext.CurrencyRates
                    .Where(cr => cr.Currency == currencyCode.ToUpper())
                    .Where(cr => cr.DateTime > limitTime)
                    .CountAsync();
                if (count == 0)
                    return null;
            }

            return average;
        }
    }
}
