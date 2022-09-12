using CSharpFunctionalExtensions;
using FXExchange.Business.MoneyConverter;
using FXExchange.Common.Models;
using FXExchange.DB.ExchangeRates;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace FxExchange.UnitTests.FXExchange.Business
{
    public class MoneyConverterServiceTests
    {
        private IMoneyConverterService _moneyConverterService;
        private Mock<IExchangeRateRepository> _exchangeRateRepository;

        [SetUp]
        public void SetUp()
        {
            _exchangeRateRepository = new Mock<IExchangeRateRepository>();
            _moneyConverterService = new MoneyConverterService(_exchangeRateRepository.Object);
        }

        [Test]
        [TestCase("EUR", 1, "DKK", 7.4394, 7.4394)]
        public async Task ConvertsMoneyDirectlyAsync(
            string fromCurrencyIso, decimal fromAmount,
            string toCurrencyso, decimal toAmount,
            decimal multiplier)
        {
            //Arrange
            var fromMoney = new Money(new Currency(fromCurrencyIso), new PositiveDecimal(fromAmount));
            var toMoney = new Money(new Currency(toCurrencyso), new PositiveDecimal(toAmount));

            _exchangeRateRepository
                .Setup(repo => repo.FindExchangeRateAsync(
                    It.Is<Currency>(cur => CurrencyEqualityComparer.Instance.Equals(cur, fromMoney.Currency)),
                    It.Is<Currency>(cur => CurrencyEqualityComparer.Instance.Equals(cur, toMoney.Currency)),
                    It.IsAny<bool>()))
                .Returns(async () => new ExchangeRate(fromMoney.Currency, toMoney.Currency, new PositiveDecimal(multiplier)));

            //Act
            var moneyConversionResult = await _moneyConverterService.ConvertMoneyAsync(fromMoney, toMoney.Currency);

            //Assert
            if (moneyConversionResult.IsSuccess)
            {
                var convertedMoney = moneyConversionResult.Value;
                Assert.IsTrue(
                    CurrencyEqualityComparer.Instance.Equals(convertedMoney.Currency, toMoney.Currency),
                    $"Got unexpected currency. Expected: {toMoney.Currency.IsoCode}, Actual: {convertedMoney.Currency.IsoCode}.");

                Assert.IsTrue(
                    toMoney.Amount == convertedMoney.Amount,
                    $"Got unexpected amount. Expected: {(decimal)toMoney.Amount}, Actual: {(decimal)convertedMoney.Amount}.");
            }
            else
            {
                Assert.Fail($"Failed to convert money: {moneyConversionResult.Error}");
            }
        }

        [Test]
        [TestCase(
            "ASD", 2,
            "DSA", 2 * 3 * 5,
            3, 5)]
        public async Task ConvertsMoneyIndirectlyAsync(
            string fromCurrencyIso, decimal fromAmount,
            string toCurrencyso, decimal toAmount,
            decimal multiplierToDkk, decimal multiplierFromDkk)
        {
            //Arrange
            var fromMoney = new Money(new Currency(fromCurrencyIso), new PositiveDecimal(fromAmount));
            var toMoney = new Money(new Currency(toCurrencyso), new PositiveDecimal(toAmount));
            var dkk = new Currency("DKK");

            _exchangeRateRepository
                .Setup(repo => repo.FindExchangeRateAsync(It.IsAny<Currency>(), It.IsAny<Currency>(), It.IsAny<bool>()))
                .Returns(async () => Result.Failure<ExchangeRate>("Direct conversion not found."));

            _exchangeRateRepository
                .Setup(repo => repo.GetAllExchangeRatesAsync(It.IsAny<bool>()))
                .Returns(async () => new[]
                {
                    new ExchangeRate(fromMoney.Currency, dkk, new PositiveDecimal(multiplierToDkk)),
                    new ExchangeRate(dkk, toMoney.Currency, new PositiveDecimal(multiplierFromDkk))
                });

            //Act
            var moneyConversionResult = await _moneyConverterService.ConvertMoneyAsync(fromMoney, toMoney.Currency);

            //Assert
            if (moneyConversionResult.IsSuccess)
            {
                var convertedMoney = moneyConversionResult.Value;
                Assert.IsTrue(
                    CurrencyEqualityComparer.Instance.Equals(convertedMoney.Currency, toMoney.Currency),
                    $"Got unexpected currency. Expected: {toMoney.Currency.IsoCode}, Actual: {convertedMoney.Currency.IsoCode}.");

                Assert.IsTrue(
                    toMoney.Amount == convertedMoney.Amount,
                    $"Got unexpected amount. Expected: {(decimal)toMoney.Amount}, Actual: {(decimal)convertedMoney.Amount}.");
            }
            else
            {
                Assert.Fail($"Failed to convert money: {moneyConversionResult.Error}");
            }
        }
    }
}
