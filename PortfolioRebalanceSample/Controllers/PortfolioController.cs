using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using AlphaVantage.Net.Core.Client;
using AlphaVantage.Net.Stocks;
using AlphaVantage.Net.Stocks.Client;

namespace PortfolioRebalanceSample.Controllers
{
    public class RebalanceRequest
    {
        public Dictionary<string, int>? CurrentPortfolio { get; set; }
        public Dictionary<string, double>? DesiredPortfolio { get; set; }
    }

    public class PortfolioRebalanceResult
    {
        public string? Symbol { get; set; }
        public int SharesBoughtOrSold { get; set; }
        public Decimal NewAllocationPercentage { get; set; }
        
        public Decimal ChangePercentage { get; set; }
    }

    

    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly StocksClient _alphaAdvantageClient;

        public PortfolioController(AlphaVantageClient alphaAdvantageClient)
        {
            _alphaAdvantageClient = alphaAdvantageClient.Stocks();
        }

        [HttpPost]
        public async Task<ActionResult<List<PortfolioRebalanceResult>>> Rebalance([FromBody] RebalanceRequest request)
        {
            var result = new List<PortfolioRebalanceResult>();

            Dictionary<string, GlobalQuote> stockPrices = await GetStockQuotes(request.CurrentPortfolio!.Keys.ToArray());

            // Calculate the total portfolio value
            double totalValue = request.CurrentPortfolio.Sum(x => x.Value * (double)stockPrices[x.Key].Price);

            // Calculate the number of shares to buy or sell for each stock
            foreach (var stock in request.CurrentPortfolio)
            {
                double desiredValue = totalValue * request.DesiredPortfolio![stock.Key];
                double desiredShares = desiredValue / (double)stockPrices[stock.Key].Price;
                int desiredSharesRounded = (int)Math.Round(desiredShares);
                int sharesToBuyOrSell = desiredSharesRounded - stock.Value;
                result.Add(new PortfolioRebalanceResult()
                {
                    Symbol = stock.Key,
                    SharesBoughtOrSold = sharesToBuyOrSell,
                    NewAllocationPercentage = (decimal)request.DesiredPortfolio[stock.Key],
                    ChangePercentage = stockPrices[stock.Key].ChangePercent,
                });
            }

            return result;
        }

        private async Task<Dictionary<string, GlobalQuote>> GetStockQuotes(string[] symbols)
        {
            // Call the Alpha Advantage API to get the current stock prices
            // If the API is not available, return fake data
            Dictionary<string, GlobalQuote> result = new Dictionary<string, GlobalQuote>();
           
            foreach (var symbol in symbols)
            {
                var quote = await _alphaAdvantageClient.GetGlobalQuoteAsync(symbol);

                if (quote != null)
                {
                    result.Add(symbol, quote);
                }
            }
            return result;
        }
    }
}
