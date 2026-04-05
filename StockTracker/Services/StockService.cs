namespace StockTracker.Services;

using System;
using System.Linq;
using System.Threading.Tasks;
using StockTracker.Interfaces;
using Skender.Stock.Indicators;

public class StockService
{
    private readonly IStockDataSource _dataSource;

    public StockService(IStockDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<double?> RunAsync(string ticker)
    {
        if (string.IsNullOrWhiteSpace(ticker)) return null;

        try
        {
            Console.WriteLine($"[INFO] Fetching analysis for {ticker.ToUpper()}...");

            var quotes = await _dataSource.GetQuotesAsync(ticker);

            // RSI(14) calculation via Skender library
            // Note: Requires historical data prior to the target date to stabilize
            var results = quotes.GetRsi(14);
            var latest = results.LastOrDefault();

            if (latest?.Rsi == null)
            {
                Console.WriteLine($"[INFO] insufficient data to calculate RSI for {ticker}");
                return null;
            }

            double rsi = (double)latest?.Rsi;
            Console.WriteLine($"{ticker.ToUpper()} | Date: {latest.Date:yyyy-MM-dd} | RSI: {rsi:F2}");

            // <30 Oversold, >70 Overbought
            if (rsi <= 30)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("SIGNAL: OVERSOLD (BUY)");
            }
            else if (rsi >= 70)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("SIGNAL: OVERBOUGHT (SELL)");
            }
            else
            {
                Console.WriteLine("SIGNAL: NEUTRAL (HOLD)");
            }
            Console.ResetColor();
            return latest?.Rsi;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ticker}: {ex.Message}");
            return null;
        }
    }
}