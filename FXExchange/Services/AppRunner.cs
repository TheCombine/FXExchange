using CSharpFunctionalExtensions;
using FXExchange.Business.MoneyConverter;
using FXExchange.CommandLineArguments;
using FXExchange.Common.Models;
using System;
using System.Threading.Tasks;

namespace FXExchange.Services
{
    public interface IAppRunner
    {
        Task RunAsync(string[] args);
    }

    internal class AppRunner : IAppRunner
    {
        private readonly IResultWriter _resultWriter;
        private readonly IArgumentsParser _argumentsParser;
        private readonly IMoneyConverterService _moneyConverterService;

        public AppRunner(
            IResultWriter resultWriter,
            IArgumentsParser argumentsParser,
            IMoneyConverterService moneyConverterService)
        {
            _resultWriter = resultWriter;
            _argumentsParser = argumentsParser;
            _moneyConverterService = moneyConverterService;
        }

        public async Task RunAsync(string[] args)
        {
            if (args.Length != 2)
            {
                _resultWriter.WriteLine("Usage: Exchange <currency pair> <amount to exchange>");
                return;
            }

            var currencyPairStr = args[0];
            var amountToExchangeStr = args[1];

            var convertedMoneyResult = await _argumentsParser.ParseCurrencyPair(currencyPairStr)
                .Bind(currencyPair => _argumentsParser.ParseAmountToExchange(amountToExchangeStr)
                    .Map(amountToExchange => new Money(currencyPair.FromCurrency, amountToExchange))
                    .Bind(moneyToExchange => _moneyConverterService.ConvertMoneyAsync(moneyToExchange, currencyPair.ToCurrency)));

            if (convertedMoneyResult.IsFailure)
            {
                _resultWriter.WriteLine($"Failed to convert money:{Environment.NewLine}{convertedMoneyResult.Error}");
                return;
            }
            var convertedMoney = convertedMoneyResult.Value;

            decimal amount = convertedMoney.Amount;
            _resultWriter.WriteLine(amount.ToString("0.####"));
        }
    }
}
