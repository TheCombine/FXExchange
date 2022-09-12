using FXExchange.Common.Models;
using FXExchange.DB.Currencies;

namespace FXExchange.DB.ExchangeRates
{
    internal static class ExchangeRateMapper
    {
        public static ExchangeRate MapToExchangeRate(this ExchangeRateEntity entity)
        {
            return new ExchangeRate(
                entity.FromCurrency.MapToCurrency(),
                entity.ToCurrency.MapToCurrency(),
                new PositiveDecimal(entity.ToAmount / entity.FromAmount));
        }
    }
}
