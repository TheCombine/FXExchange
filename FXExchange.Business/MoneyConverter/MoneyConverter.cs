using FXExchange.Common.Models;
using System;

namespace FXExchange.Business.MoneyConverter
{
    internal class MoneyConverter
    {
        private readonly ExchangeRate _exchangeRate;

        public MoneyConverter(ExchangeRate exchangeRate)
        {
            _exchangeRate = exchangeRate;
        }

        public Money Convert(Money money)
        {
            if (!CurrencyEqualityComparer.Instance.Equals(money.Currency, _exchangeRate.FromCurrency))
                throw new InvalidOperationException();

            var convertedAmount = money.Amount * _exchangeRate.Multiplier;
            var convertedCurrency = _exchangeRate.ToCurrency;
            return new Money(convertedCurrency, new PositiveDecimal(convertedAmount));
        }
    }
}
