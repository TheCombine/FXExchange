using Autofac;
using FXExchange.Business.MoneyConverter;
using FXExchange.CommandLineArguments;
using FXExchange.Services;
using Moq;
using NUnit.Framework;
using System;

namespace FxExchange.UnitTests.FXExchange
{
    public class AppRunnerTests : BaseTestContainer
    {
        private Mock<IResultWriter> _resultWriter;
        private IAppRunner _appRunner;

        [SetUp]
        public void SetUp()
        {
            _resultWriter = new Mock<IResultWriter>();
            _appRunner = new AppRunner(_resultWriter.Object, Container.Resolve<IArgumentsParser>(), Container.Resolve<IMoneyConverterService>());
        }

        [Test]
        public void InformsOfCommandUsage()
        {
            // Arrange
            _resultWriter.Setup(rw => rw.WriteLine(It.IsAny<string>()));

            // Act
            _appRunner.RunAsync(Array.Empty<string>());

            // Assert
            _resultWriter.Verify(rw => rw.WriteLine(It.Is<string>(val => val == "Usage: Exchange <currency pair> <amount to exchange>")));
        }

        [Test]
        [TestCase("EUR/DKK 1", "7.4394")]
        public void ConvertsKnownCurrencies(string args, string expected)
        {
            // Arrange
            var splitArgs = args.Split();
            _resultWriter.Setup(rw => rw.WriteLine(It.IsAny<string>()));

            // Act
            _appRunner.RunAsync(splitArgs);

            // Assert
            _resultWriter.Verify(rw => rw.WriteLine(It.Is<string>(val => val == expected)));
        }

        [Test]
        [TestCase("EUR/EUR 1.23")]
        [TestCase("DKK/DKK 13")]
        [TestCase("SEK/SEK 1000")]
        public void ConvertsCurrencyToSelf(string args)
        {
            // Arrange
            var splitArgs = args.Split();
            var amount = splitArgs[1];
            _resultWriter.Setup(rw => rw.WriteLine(It.IsAny<string>()));

            // Act
            _appRunner.RunAsync(splitArgs);

            // Assert
            _resultWriter.Verify(rw => rw.WriteLine(It.Is<string>(val => val == amount)));
        }

        [TestCase("EUR/LTL 0")]
        [TestCase("LTL/EUR 0")]
        public void DoesNotConvertBetweenKnownAndUnknownCurrencies(string args)
        {
            // Arrange
            var splitArgs = args.Split();
            var amount = splitArgs[1];
            _resultWriter.Setup(rw => rw.WriteLine(It.IsAny<string>()));

            // Act
            _appRunner.RunAsync(splitArgs);

            // Assert
            _resultWriter.Verify(rw => rw.WriteLine(It.Is<string>(val => val.StartsWith("Failed to convert money:"))));
        }
    }
}
