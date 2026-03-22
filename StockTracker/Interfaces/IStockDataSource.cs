namespace StockTracker.Interfaces;
using Skender.Stock.Indicators;
public interface IStockDataSource
{
	Task <IEnumerable<Quote>> GetQuotesAsync(string symbol);
}