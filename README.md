# StockTracker
A quick tool to check if a stock is oversold/overbought using RSI indicator.
It uses the Alpha Vantage API to get the stock data.

How to use:

1.) Get an API key from Alpha Vantage: https://www.alphavantage.co/support/#api-key

2.) Run <dotnet user-secrets set "ALPHAVANTAGE_API_KEY" "your_api_key_here"> in the terminal to set the API key as a user secret

3.) Run the application <dotnet run> and follow the prompts to enter a stock symbol to calculate the RSI

The Tech Stack:
- C# / .NET 10
- Skender.Stock.Indicators: Handles the RSI math so I don't have to.
- AlphaVantage.Net: For the price data.
- Dependency Injection: Can swap the real API for a Mock source (to save AlphaV API credits when testing)
