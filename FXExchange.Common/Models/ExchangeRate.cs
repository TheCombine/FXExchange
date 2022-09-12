namespace FXExchange.Common.Models
{
    public readonly struct ExchangeRate
    {
        public Currency FromCurrency { get; }
        public Currency ToCurrency { get; }
        public PositiveDecimal Multiplier { get; }

        public ExchangeRate(Currency fromCurrency, Currency toCurrency, PositiveDecimal multiplier)
        {
            FromCurrency = fromCurrency;
            ToCurrency = toCurrency;
            Multiplier = multiplier;
        }

        public bool CanConvert(Currency fromCurrency)
        {
            return CurrencyEqualityComparer.Instance.Equals(FromCurrency, fromCurrency);
        }

        public override string ToString()
        {
            return $"1 {FromCurrency.IsoCode} -> {(decimal)Multiplier} {ToCurrency.IsoCode}";
        }
    }
}
