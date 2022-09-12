using CSharpFunctionalExtensions;
using FXExchange.Common.Models;
using System.Text.RegularExpressions;

namespace FXExchange.CommandLineArguments
{
    public interface IArgumentsParser
    {
        Result<CurrencyPair> ParseCurrencyPair(string currencyPair);
        Result<PositiveDecimal> ParseAmountToExchange(string amountToExchange);
    }

    internal class ArgumentsParser : IArgumentsParser
    {
        public Result<CurrencyPair> ParseCurrencyPair(string currencyPair)
        {
            currencyPair = currencyPair.ToUpper();

            var match = Regex.Match(
                input: currencyPair,
                pattern: @"^(?<fromCurrency>[A-Z]{3})/(?<toCurrency>[A-Z]{3})$");

            if (!match.Success)
                return Result.Failure<CurrencyPair>("Currency pair format error.");

            var fromCurrency = match.Groups["fromCurrency"].Value;
            var toCurrency = match.Groups["toCurrency"].Value;

            return new CurrencyPair(new Currency(fromCurrency), new Currency(toCurrency));
        }

        public Result<PositiveDecimal> ParseAmountToExchange(string amountToExchange)
        {
            if (!decimal.TryParse(amountToExchange, out var parsedAmount))
                return Result.Failure<PositiveDecimal>("Amount to exchange format error.");
            if (parsedAmount < 0)
                return Result.Failure<PositiveDecimal>("Amount to exchange must be a positive number");

            return new PositiveDecimal(parsedAmount);
        }
    }
}
