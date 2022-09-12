namespace FXExchange.Common.Models
{
    public readonly struct Money
    {
        public Currency Currency { get; }
        public PositiveDecimal Amount { get; }

        public Money(Currency currency, PositiveDecimal amount)
        {
            Currency = currency;
            Amount = amount;
        }
    }
}