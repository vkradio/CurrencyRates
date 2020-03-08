using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers.Api
{
    [Route("api/currency-rates")]
    //[AutoValidateAntiforgeryToken]
    public class CurrencyRateController : Controller
    {
        ICurrencyRateRepository repository;

        public CurrencyRateController(ICurrencyRateRepository repository) => this.repository = repository;

        [HttpGet("{currencyCode}")]
        public async Task<IEnumerable<CurrencyRate>> Get(string currencyCode) =>
            await repository.ListAsync(new RateOfCurrencySpecification(currencyCode));

        [HttpGet("{currencyCode}/avg24h")]
        public async Task<IActionResult> GetAvg24h(string currencyCode)
        {
            var average = await repository.GetAverageRateBy24hAsync(currencyCode);
            return average.HasValue ? (IActionResult)Ok(average) : NotFound();
        }
    }
}
