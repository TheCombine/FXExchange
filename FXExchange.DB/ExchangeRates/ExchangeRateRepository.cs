using CSharpFunctionalExtensions;
using FXExchange.Common.Models;
using FXExchange.DB.Currencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FXExchange.DB.ExchangeRates
{
    public interface IExchangeRateRepository
    {
        Task<Result<ExchangeRate>> FindExchangeRateAsync(
            Currency fromCurrency, Currency toCurrency, bool allowReverseConversion = true);
        Task<Result<IReadOnlyCollection<ExchangeRate>>> GetAllExchangeRatesAsync(
            bool allowReverseConversion = true);
    }

    internal class ExchangeRateRepository : IExchangeRateRepository
    {
        internal static readonly IEnumerable<ExchangeRateEntity> ExchangeRates = GetExchangeRates();
        private static IEnumerable<ExchangeRateEntity> GetExchangeRates()
        {
            var exchangeRates = new List<ExchangeRateEntity>();
            AddExchangeRate("DKK", 743.94m, "EUR", 100);
            AddExchangeRate("DKK", 663.11m, "USD", 100);
            AddExchangeRate("DKK", 852.85m, "GBP", 100);
            AddExchangeRate("DKK", 76.10m, "SEK", 100);
            AddExchangeRate("DKK", 78.40m, "NOK", 100);
            AddExchangeRate("DKK", 683.58m, "CHF", 100);
            AddExchangeRate("DKK", 5.9740m, "JPY", 100);
            return exchangeRates;

            void AddExchangeRate(string fromIsoCode, decimal fromAmount, string toIsoCode, decimal toAmount)
            {
                exchangeRates.Add(new ExchangeRateEntity
                {
                    FromCurrency = GetCurrency(fromIsoCode),
                    FromAmount = fromAmount,
                    ToCurrency = GetCurrency(toIsoCode),
                    ToAmount = toAmount,
                });

                CurrencyEntity GetCurrency(string isoCode)
                    => CurrencyRepository.Currencies.Single(currency => currency.IsoCode == isoCode);
            }
        }

        public Task<Result<ExchangeRate>> FindExchangeRateAsync(
            Currency fromCurrency, Currency toCurrency, bool allowReverseConversion = true)
        {
            return FindExchangeRateAsync(fromCurrency, toCurrency)
                .OnFailureCompensate(error =>
                {
                    if (allowReverseConversion)
                    {
                        return FindExchangeRateAsync(fromCurrency: toCurrency, toCurrency: fromCurrency).Map(ReverseExchangeRate);
                    }
                    else
                    {
                        return Task.FromResult(Result.Failure<ExchangeRate>(error));
                    }
                });
            
            async Task<Result<ExchangeRate>> FindExchangeRateAsync(Currency fromCurrency, Currency toCurrency)
            {
                Maybe<ExchangeRateEntity> entity;
                var fromCurrencyIso = fromCurrency.IsoCode;
                var toCurrencyIso = toCurrency.IsoCode;

                try
                {
                    entity = ExchangeRates.SingleOrDefault(exchangeRate
                        => string.Equals(exchangeRate.FromCurrency.IsoCode, fromCurrencyIso, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(exchangeRate.ToCurrency.IsoCode, toCurrencyIso, StringComparison.OrdinalIgnoreCase));
                }
                catch (Exception ex)
                {
                    // _logger.LogError("Error occured when querying database.", ex);
                    return Result.Failure<ExchangeRate>("Error fetching ExchangeRate from database.");
                }

                return entity.ToResult("ExchangeRate not found in database.").Map(e => e.MapToExchangeRate());
            }
        }

        public Task<Result<IReadOnlyCollection<ExchangeRate>>> GetAllExchangeRatesAsync(bool allowReverseConversion = true)
        {
            return GetAllExchangeRatesAsync().Map(exchangeRates =>
            {
                if (allowReverseConversion)
                {
                    var reverseExchangeRates = exchangeRates.Select(ReverseExchangeRate);
                    return exchangeRates.Concat(reverseExchangeRates).ToArray();
                }
                else
                {
                    return exchangeRates;
                }
            });

            async Task<Result<IReadOnlyCollection<ExchangeRate>>> GetAllExchangeRatesAsync()
            {
                try
                {
                    return ExchangeRates.AsEnumerable().Select(exchangeRate => exchangeRate.MapToExchangeRate()).ToArray();
                }
                catch (Exception ex)
                {
                    // _logger.LogError("Error occured when querying database.", ex);
                    return Result.Failure<IReadOnlyCollection<ExchangeRate>>("Error fetching ExchangeRates from database.");
                }
            }
        }

        private static ExchangeRate ReverseExchangeRate(ExchangeRate exchangeRate)
        {
            return new ExchangeRate(
                fromCurrency: exchangeRate.ToCurrency, toCurrency: exchangeRate.FromCurrency, multiplier: new PositiveDecimal(1 / exchangeRate.Multiplier));
        }
    }
}
