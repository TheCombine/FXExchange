using System;

namespace FXExchange.Common.Models
{
    public readonly struct PositiveDecimal
    {
        private readonly decimal _value;

        public PositiveDecimal(decimal value)
        {
            if (value < 0)
                throw new ArgumentException($"Value must be positive. Value: '{value}'.", nameof(value));

            _value = value;
        }

        public static implicit operator decimal(PositiveDecimal moneyAmount)
        {
            return moneyAmount._value;
        }
    }
}