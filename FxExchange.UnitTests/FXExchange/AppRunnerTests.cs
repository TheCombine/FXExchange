using FXExchange.Business.MoneyConverter;
using FXExchange.CommandLineArguments;
using FXExchange.Services;
using Moq;
using NUnit.Framework;

namespace FxExchange.UnitTests.FXExchange
{
    public class AppRunnerTests
    {
        private IAppRunner _appRunner;
        private Mock<IResultWriter> _resultWriter;
        private Mock<IArgumentsParser> _argumentsParser;
        private Mock<IMoneyConverterService> _moneyConverterService;

        [SetUp]
        public void SetUp()
        {
            _resultWriter = new Mock<IResultWriter>();
            _argumentsParser = new Mock<IArgumentsParser>();
            _moneyConverterService = new Mock<IMoneyConverterService>();
            _appRunner = new AppRunner(_resultWriter.Object, _argumentsParser.Object, _moneyConverterService.Object);
        }

        [Test]
        [TestCase("help")]
        [TestCase("")]
        [TestCase("EUR/EUR 1")]
        public void WritesSomethingToTheResultWriterOnce(string args)
        {
            // Arrange
            _resultWriter.Setup(rw => rw.WriteLine(It.IsAny<string>()));
            var splitArgs = args.Split();

            // Act
            _appRunner.RunAsync(splitArgs);

            // Asser
            _resultWriter.Verify(rw => rw.WriteLine(It.IsAny<string>()), Times.Once);
        }
    }
}
