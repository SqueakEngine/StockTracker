namespace StockTracker.DataSources;

using System;
using System.Net.Http;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockTracker.Interfaces;
using Skender.Stock.Indicators;

public class AlphaVantageSource : IStockDataSource
{
    private readonly string _apiKey;
    private static readonly HttpClient _httpClient = new HttpClient();

    public AlphaVantageSource(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<IEnumerable<Quote>> GetQuotesAsync(string symbol)
    {
        Console.WriteLine($"Fetching data for {symbol} from Alpha Vantage...");
        string url = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={_apiKey}";

        //go to AlphaVantage and get the JSON response
        var response = await _httpClient.GetStringAsync(url);
        // Parse the JSON and convert it to a list of Quote objects
        using var doc = JsonDocument.Parse(response);

        var timeSeries = doc.RootElement.GetProperty("Time Series (Daily)");

        return timeSeries.EnumerateObject().Select(entry => new Quote
        {
            Date = DateTime.Parse(entry.Name),
            Close = decimal.Parse(entry.Value.GetProperty("4. close").GetString()!)
        }).ToList();
    }
}