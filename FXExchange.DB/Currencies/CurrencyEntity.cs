using System;

namespace FXExchange.DB.Currencies
{
    public class CurrencyEntity
    {
        // Primary key
        public string EntityId { get; private set; } = Guid.NewGuid().ToString();

        // Unique, indexed, required, length=3
        public string IsoCode { get; set; }
    }
}
