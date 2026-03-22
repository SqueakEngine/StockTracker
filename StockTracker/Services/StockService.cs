namespace StockTracker.Services;

using System;
using System.Linq;
using System.Threading.Tasks;
using StockTracker.Interfaces; 
using Skender.Stock.Indicators;

public class StockService
{
    private readonly IStockDataSource _source;

    public StockService(IStockDataSource source)
    {
        _source = source;
    }

    public async Task RunAsync(string ticker)
    {
        try
        {
            Console.WriteLine($"--- Fetching data for {ticker} ---");

            var quotes = await _source.GetQuotesAsync(ticker);
            var rsiResults = quotes.GetRsi(14);
            var latest = rsiResults.LastOrDefault();

            if (latest?.Rsi != null)
            {
                double rsiValue = (double)latest.Rsi;
                Console.WriteLine($"\n📈 Ticker: {ticker} | RSI: {rsiValue:F2}");

                if (rsiValue <= 30)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OPPORTUNITY: Oversold (BUY SIGNAL)");
                }
                else if (rsiValue >= 70)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("WARNING: Overbought (SELL SIGNAL)");
                }
                else
                {
                    Console.WriteLine("STATUS: Neutral (HOLD)");
                }
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ CRITICAL ERROR: {ex.Message}");
        }
    }
}