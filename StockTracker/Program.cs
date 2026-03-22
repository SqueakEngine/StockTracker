using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockTracker.Interfaces;
using StockTracker.DataSources;
using AlphaVantage.Net.Stocks; 
using Skender.Stock.Indicators;

// ==========================================
// 1. EXECUTION LOGIC (The "Engine")
// ==========================================

Console.WriteLine("🚀 Starting RSI Swing Trade Monitor...");

// SWITCH HERE: Use MockStockSource() to test without hitting API limits
// Use AlphaVantageSource("YOUR_KEY") for real data
string? apiKey = Environment.GetEnvironmentVariable("ALPHAVANTAGE_API_KEY");
IStockDataSource source;
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("⚠️  No API key found. Using mock data source.");
    source = new MockStockSource();
}
else
{
    Console.WriteLine("✅ API key found. Using Alpha Vantage data source.");
    source = new AlphaVantageSource(apiKey);
}

string ticker = "ASTS";

try
{
    Console.WriteLine($"--- Fetching data for {ticker} ---");

    // Fetch quotes from our abstracted source
    var quotes = await source.GetQuotesAsync(ticker);

    // Calculate RSI using a 14-period lookback
    var rsiResults = quotes.GetRsi(14);
    var latest = rsiResults.LastOrDefault();

    if (latest?.Rsi != null)
    {
        double rsiValue = (double)latest.Rsi;
        Console.WriteLine($"\n📈 Ticker: {ticker}");
        Console.WriteLine($"📅 Date: {latest.Date:yyyy-MM-dd}");
        Console.WriteLine($"📊 RSI: {rsiValue:F2}");

        // Swing Trade Alert Logic
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

Console.WriteLine("\n--- Monitor Cycle Complete ---");