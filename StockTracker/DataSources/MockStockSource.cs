namespace StockTracker.DataSources; // <--- This MUST be line 1

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockTracker.Interfaces;
using Skender.Stock.Indicators;

public class MockStockSource : IStockDataSource
{
    public async Task<IEnumerable<Quote>> GetQuotesAsync(string symbol)
    {
        await Task.Delay(100);
        return Enumerable.Range(0, 30).Select(i => new Quote
        {
            Date = DateTime.Now.AddDays(-i),
            Close = 150 + (decimal)i // Prices trending up
        }).ToList();
    }
}