using FXExchange.Common.Models;

namespace FXExchange.DB.Currencies
{
    internal static class CurrencyMapper
    {
        public static Currency MapToCurrency(this CurrencyEntity entity)
        {
            return new Currency(entity.IsoCode);
        }
    }
}
