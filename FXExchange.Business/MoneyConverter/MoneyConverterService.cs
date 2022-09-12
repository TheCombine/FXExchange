using CSharpFunctionalExtensions;
using FXExchange.Common.Models;
using FXExchange.DB.ExchangeRates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FXExchange.Business.MoneyConverter
{
    public interface IMoneyConverterService
    {
        Task<Result<Money>> ConvertMoneyAsync(Money fromMoney, Currency toCurrency, bool allowMultipleConversions = true);
    }

    internal class MoneyConverterService : IMoneyConverterService
    {
        private readonly IExchangeRateRepository _exchangeRateRepository;

        public MoneyConverterService(
            IExchangeRateRepository exchangeRateRepository)
        {
            _exchangeRateRepository = exchangeRateRepository;
        }

        public Task<Result<Money>> ConvertMoneyAsync(Money fromMoney, Currency toCurrency, bool allowMultipleConversions = true)
        {
            if (CurrencyEqualityComparer.Instance.Equals(fromMoney.Currency, toCurrency))
                return Task.FromResult<Result<Money>>(fromMoney);

            return _exchangeRateRepository.FindExchangeRateAsync(fromMoney.Currency, toCurrency)
                .OnFailureCompensate(error =>
                {
                    if (allowMultipleConversions)
                    {
                        return _exchangeRateRepository.GetAllExchangeRatesAsync().Bind(exchangeRates => GetBestDerivedExchangeRate(exchangeRates, fromMoney.Currency, toCurrency).ToResult("No exchange rate could be derived."));
                    }
                    else
                    {
                        return Task.FromResult(Result.Failure<ExchangeRate>(error));
                    }
                })
                .Map(exchangeRate => new MoneyConverter(exchangeRate).Convert(fromMoney));
        }

        private static Maybe<ExchangeRate> GetBestDerivedExchangeRate(IEnumerable<ExchangeRate> exchangeRates, Currency fromCurrency, Currency toCurrency)
        {
            var bestDerivedExchangeRate = DeriveExchangeRates(exchangeRates, fromCurrency, toCurrency).OrderByDescending(exchangeRate => (decimal)exchangeRate.Multiplier).Select(exchangeRate => Maybe.From(exchangeRate)).FirstOrDefault();
            return bestDerivedExchangeRate.Map(rate => new ExchangeRate(rate.FromCurrency, rate.ToCurrency, rate.Multiplier));

            static IEnumerable<DerivedExchangeRate> DeriveExchangeRates(IEnumerable<ExchangeRate> exchangeRates, Currency fromCurrency, Currency toCurrency)
            {
                var applicableExchangeRates = exchangeRates.Where(exchangeRate => exchangeRate.CanConvert(fromCurrency));
                foreach (var exchangeRate in applicableExchangeRates)
                {
                    var dontConvertTo = new HashSet<Currency>(CurrencyEqualityComparer.Instance);
                    dontConvertTo.Add(exchangeRate.FromCurrency);
                    foreach (var derivedExchangeRate in DeriveExchangeRatesInternal(new DerivedExchangeRate(exchangeRate), toCurrency, dontConvertTo, exchangeRates))
                    {
                        yield return derivedExchangeRate;
                    }
                    dontConvertTo.Remove(exchangeRate.FromCurrency);
                }

                static IEnumerable<DerivedExchangeRate> DeriveExchangeRatesInternal(
                    DerivedExchangeRate currentExchangeRate,
                    Currency targetCurrency,
                    ISet<Currency> dontConvertTo,
                    IEnumerable<ExchangeRate> allExchangeRates)
                {
                    if (CurrencyEqualityComparer.Instance.Equals(currentExchangeRate.ToCurrency, targetCurrency))
                    {
                        yield return currentExchangeRate;
                        yield break;
                    }

                    var applicableExchangeRates = allExchangeRates.Where(exchangeRate => exchangeRate.CanConvert(currentExchangeRate.ToCurrency) && !dontConvertTo.Contains(exchangeRate.ToCurrency));
                    foreach (var exchangeRate in applicableExchangeRates)
                    {
                        var derivedExchangeRate = currentExchangeRate.Clone();
                        derivedExchangeRate.AddExchangeRate(exchangeRate);

                        dontConvertTo.Add(exchangeRate.ToCurrency);
                        foreach (var derivedExchangeRateInternal in DeriveExchangeRatesInternal(derivedExchangeRate, targetCurrency, dontConvertTo, allExchangeRates))
                        {
                            if (CurrencyEqualityComparer.Instance.Equals(derivedExchangeRateInternal.ToCurrency, targetCurrency))
                            {
                                yield return derivedExchangeRateInternal;
                            }
                        }
                        dontConvertTo.Remove(exchangeRate.ToCurrency);
                    }
                }
            }
        }

        private class DerivedExchangeRate
        {
            private readonly List<ExchangeRate> _exchangeChain = new List<ExchangeRate>();
            public IReadOnlyList<ExchangeRate> ExchangeChain => _exchangeChain;

            public Currency FromCurrency => ExchangeChain.First().FromCurrency;
            public Currency ToCurrency => ExchangeChain.Last().ToCurrency;
            public PositiveDecimal Multiplier => new PositiveDecimal(ExchangeChain.Aggregate(1m, (accumulator, current) => accumulator * current.Multiplier));

            private DerivedExchangeRate()
            {
            }

            public DerivedExchangeRate(ExchangeRate initialExchangeRate)
            {
                _exchangeChain.Add(initialExchangeRate);
            }

            public void AddExchangeRate(ExchangeRate exchangeRate)
            {
                if (exchangeRate.CanConvert(ToCurrency))
                {
                    _exchangeChain.Add(exchangeRate);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Can't add to exchange chain. Chain's ToCurrency: {ToCurrency.IsoCode}, trying to add ExchangeRate with FromCurrency: {exchangeRate.FromCurrency.IsoCode}");
                }
            }

            public DerivedExchangeRate Clone()
            {
                var clone = new DerivedExchangeRate();
                clone._exchangeChain.AddRange(_exchangeChain);
                return clone;
            }
        }
    }
}
