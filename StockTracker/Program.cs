using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using StockTracker.Interfaces;
using StockTracker.DataSources;
using StockTracker.Services;
using AlphaVantage.Net.Stocks;
using Skender.Stock.Indicators;

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

string? apiKey = config["ALPHAVANTAGE_API_KEY"];

var services = new ServiceCollection();

if (string.IsNullOrEmpty(apiKey))
{
    services.AddSingleton<IStockDataSource, MockStockSource>();
}
else
{
    services.AddSingleton<IStockDataSource>(sp => new AlphaVantageSource(apiKey));
}

services.AddTransient<StockService>();

var serviceProvider = services.BuildServiceProvider();

var engine = serviceProvider.GetRequiredService<StockService>();
Console.WriteLine("Enter a stock ticker symbol (e.g., SHLS) or press Enter to use default:");
//Default to VOO if no input
await engine.RunAsync(Console.ReadLine() ?? "VOO");
