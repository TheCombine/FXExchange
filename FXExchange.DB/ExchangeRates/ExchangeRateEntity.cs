using FXExchange.DB.Currencies;
using System;

namespace FXExchange.DB.ExchangeRates
{
    internal class ExchangeRateEntity
    {
        // Primary key
        public string EntityId { get; private set; } = Guid.NewGuid().ToString();

        // Indexed, foreign key, required
        public CurrencyEntity FromCurrency { get; set; }
        public decimal FromAmount { get; set; }

        // Indexed, foreign key, required
        public CurrencyEntity ToCurrency { get; set; }
        public decimal ToAmount { get; set; }
    }
}
