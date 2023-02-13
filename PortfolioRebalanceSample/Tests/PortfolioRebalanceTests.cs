using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using PortfolioRebalanceSample.Controllers;
using AlphaVantage.Net.Stocks;
using AlphaVantage.Net.Stocks.Client;
using AlphaVantage.Net.Core.Client;

namespace PortfolioRebalanceSample.Tests
{
    public class PortfolioControllerTests
    {
        [Fact]
        public async Task Rebalance_ReturnsExpectedResult()
        {
            // Arrange
            var request = new RebalanceRequest
            {
                CurrentPortfolio = new Dictionary<string, int>
                {
                    { "AAPL", 100 },
                    { "GOOG", 50 }
                },
                DesiredPortfolio = new Dictionary<string, double>
                {
                    { "AAPL", 0.6 },
                    { "GOOG", 0.4 }
                }
            };

            var globalQuoteAAPL = new GlobalQuote
            {
                Symbol = "AAPL",
                Price = 150,
                ChangePercent = 0.5M
            };

            var globalQuoteGOOG = new GlobalQuote
            {
                Symbol = "GOOG",
                Price = 1200,
                ChangePercent = -0.3M
            };

            var stocksClientMock = new Mock<StocksClient>(MockBehavior.Strict);
            stocksClientMock
                .Setup(x => x.GetGlobalQuoteAsync("AAPL"))
                .ReturnsAsync(globalQuoteAAPL);

            stocksClientMock
                .Setup(x => x.GetGlobalQuoteAsync("GOOG"))
                .ReturnsAsync(globalQuoteGOOG);

            var alphaVantageClientMock = new Mock<AlphaVantageClient>(MockBehavior.Strict);
            alphaVantageClientMock
                .Setup(x => x.Stocks())
                .Returns(stocksClientMock.Object);

            var controller = new PortfolioController(alphaVantageClientMock.Object);

            // Act
            var result = await controller.Rebalance(request);

            // Assert
            var resultList = Assert.IsType<List<PortfolioRebalanceResult>>(result);
            Assert.Equal(2, resultList.Count);

            var aaplResult = resultList.Single(x => x.Symbol == "AAPL");
            Assert.Equal(60, aaplResult.SharesBoughtOrSold);
            Assert.Equal(0.6M, aaplResult.NewAllocationPercentage);
            Assert.Equal(0.5M, aaplResult.ChangePercentage);

            var googResult = resultList.Single(x => x.Symbol == "GOOG");
            Assert.Equal(-5, googResult.SharesBoughtOrSold);
            Assert.Equal(0.4M, googResult.NewAllocationPercentage);
            Assert.Equal(-0.3M, googResult.ChangePercentage);

            stocksClientMock.VerifyAll();
            alphaVantageClientMock.VerifyAll();
        }
    }
}
