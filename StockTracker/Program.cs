using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Your Custom Namespaces
using StockTracker.Interfaces;
using StockTracker.DataSources;
using AlphaVantage.Net.Stocks;
using Skender.Stock.Indicators;

// ==========================================
// 1. CONFIGURATION SETUP
// ==========================================
var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    // This allows you to use 'dotnet user-secrets set' locally
    .AddUserSecrets<Program>()
    .Build();

string? apiKey = config["ALPHAVANTAGE_API_KEY"];

// ==========================================
// 2. DEPENDENCY INJECTION CONTAINER
// ==========================================
var services = new ServiceCollection();

// Logic to decide which Data Source to "Inject"
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("⚠️ No API key found. Wiring up MockStockSource.");
    services.AddSingleton<IStockDataSource, MockStockSource>();
}
else
{
    Console.WriteLine("✅ API key found. Wiring up AlphaVantageSource.");
    // We pass the key into the constructor manually here during registration
    services.AddSingleton<IStockDataSource>(sp => new AlphaVantageSource(apiKey));
}

// Register our engine
services.AddTransient<StockEngine>();

var serviceProvider = services.BuildServiceProvider();

// ==========================================
// 3. EXECUTION
// ==========================================
Console.WriteLine("🚀 Starting RSI Swing Trade Monitor...");

var engine = serviceProvider.GetRequiredService<StockEngine>();
await engine.RunAsync("SHLS");

Console.WriteLine("\n--- Monitor Cycle Complete ---");

// ==========================================
// 4. THE ENGINE CLASS (Refactored Logic)
// ==========================================
public class StockEngine
{
    private readonly IStockDataSource _source;

    public StockEngine(IStockDataSource source)
    {
        _source = source;
    }

    public async Task RunAsync(string ticker)
    {
        try
        {
            Console.WriteLine($"--- Fetching data for {ticker} ---");

            // Fetch quotes from our abstracted source
            var quotes = await _source.GetQuotesAsync(ticker);

            // Calculate RSI using a 14-period lookback (Skender.Stock.Indicators)
            var rsiResults = quotes.GetRsi(14);
            var latest = rsiResults.LastOrDefault();

            if (latest?.Rsi != null)
            {
                double rsiValue = (double)latest.Rsi;
                Console.WriteLine($"\n📈 Ticker: {ticker}");
                Console.WriteLine($"📅 Date: {latest.Date:yyyy-MM-dd}");
                Console.WriteLine($"📊 RSI: {rsiValue:F2}");

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