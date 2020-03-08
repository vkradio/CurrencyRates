using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using System;
using System.Linq.Expressions;

namespace ApplicationCore.Specifications
{
    public class RateOfCurrencySpecification : BaseSpecification<CurrencyRate>
    {
        public RateOfCurrencySpecification(string currencyCode) :
            base(cr => cr.Currency == (currencyCode ?? "").ToUpper())
        { }

        public RateOfCurrencySpecification(string currencyCode, DateTime minDateTimeExclusive) :
            base(cr => cr.Currency == (currencyCode ?? "").ToUpper() && cr.DateTime > minDateTimeExclusive)
        { }
    }
}
