using System;

namespace ApplicationCore.Entities
{
    public class Currency
    {
        public const string BTC = nameof(BTC);
        public const string ETH = nameof(ETH);
    }

    public class CurrencyRate : BaseEntity
    {
        public DateTime DateTime { get; set; }
        public string Currency { get; set; }
        public decimal Rate { get; set; }
    }
}
