using ApplicationCore.Entities;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ICurrencyRateRepository : IRepository<CurrencyRate>, IAsyncRepository<CurrencyRate>
    {
        decimal? GetAverageRateBy24h(string currencyCode);
        Task<decimal?> GetAverageRateBy24hAsync(string currencyCode);
    }
}
