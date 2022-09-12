using FXExchange.Common.Models;

namespace FXExchange.CommandLineArguments
{
    public readonly struct CurrencyPair
    {
        public Currency FromCurrency { get; }
        public Currency ToCurrency { get; }

        public CurrencyPair(Currency fromCurrency, Currency toCurrency)
        {
            FromCurrency = fromCurrency;
            ToCurrency = toCurrency;
        }
    }
}
