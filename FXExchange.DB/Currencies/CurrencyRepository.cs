using System.Collections.Generic;
using System.Linq;

namespace FXExchange.DB.Currencies
{
    public interface ICurrencyRepository
    {
    }

    internal class CurrencyRepository : ICurrencyRepository
    {
        internal static readonly IEnumerable<CurrencyEntity> Currencies = new List<CurrencyEntity>
        {
            new CurrencyEntity { IsoCode = "DKK" },
            new CurrencyEntity { IsoCode = "EUR" },
            new CurrencyEntity { IsoCode = "USD" },
            new CurrencyEntity { IsoCode = "GBP" },
            new CurrencyEntity { IsoCode = "SEK" },
            new CurrencyEntity { IsoCode = "NOK" },
            new CurrencyEntity { IsoCode = "CHF" },
            new CurrencyEntity { IsoCode = "JPY" },
        };
    }
}
