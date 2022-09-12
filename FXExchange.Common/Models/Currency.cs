using System;
using System.Collections.Generic;

namespace FXExchange.Common.Models
{
    public readonly struct Currency
    {
        public string IsoCode { get; }

        public Currency(string isoCode)
        {
            if (isoCode?.Length != 3)
                throw new ArgumentException($"Invalid currency iso code: '{isoCode}'.", nameof(isoCode));

            IsoCode = isoCode.ToUpper();
        }
    }

    public class CurrencyEqualityComparer : EqualityComparer<Currency>
    {
        public static readonly CurrencyEqualityComparer Instance = new CurrencyEqualityComparer();

        private CurrencyEqualityComparer()
        {
        }

        public override bool Equals(Currency x, Currency y)
        {
            if (x.IsoCode is null && y.IsoCode is null)
                return true;

            if (x.IsoCode is null || y.IsoCode is null)
                return false;

            return x.IsoCode.Equals(y.IsoCode);
        }

        public override int GetHashCode(Currency obj)
        {
            return obj.IsoCode?.GetHashCode() ?? 0;
        }
    }
}