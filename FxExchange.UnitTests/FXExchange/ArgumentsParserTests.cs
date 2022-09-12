using FXExchange.CommandLineArguments;
using NUnit.Framework;

namespace FxExchange.UnitTests.FXExchange
{
    public class ArgumentsParserTests
    {
        private IArgumentsParser _argumentsParser;

        [SetUp]
        public void SetUp()
        {
            _argumentsParser = new ArgumentsParser();
        }

        [Test]
        [TestCase("0", 0)]
        [TestCase("1", 1)]
        [TestCase("1.23", 1.23)]
        public void ParsesPositiveAmountToExchange(string amountToExchange, decimal expectedAmount)
        {
            // Arrange

            // Act
            var amountParseResult = _argumentsParser.ParseAmountToExchange(amountToExchange);

            // Assert
            if (amountParseResult.IsSuccess)
            {
                decimal parsedAmount = amountParseResult.Value;
                Assert.AreEqual(parsedAmount, expectedAmount);
            }
            else
            {
                Assert.Fail($"Failed to parse: {amountParseResult.Error}");
            }
        }

        [Test]
        [TestCase("-1")]
        [TestCase("-1.23")]
        public void FailsToParseNegativeAmountToExchange(string amountToExchange)
        {
            // Arrange

            // Act
            var amountParseResult = _argumentsParser.ParseAmountToExchange(amountToExchange);

            // Assert
            if (amountParseResult.IsSuccess)
            {
                decimal parsedAmount = amountParseResult.Value;
                Assert.Fail($"Expected to fail. Got successfully parsed amount instead: {amountParseResult.Value}");
            }
            else
            {
                Assert.Pass();
            }
        }
    }
}
