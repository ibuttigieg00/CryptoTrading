using RatesService.EventBus;
using System.Text.Json;

namespace RatesService.Services
{
    public class RateChecker
    {
        private readonly Dictionary<string, RateHistory> _rateHistory;
        private readonly Dictionary<string, RateHistory> _rateCurrent;

        private readonly string _filePath;
        private readonly IEventBus _eventBus;

        public RateChecker(string filePath, Dictionary<string, RateHistory> rateCurrent, IEventBus eventBus)
        {
            _filePath = filePath;
            _rateHistory = new Dictionary<string, RateHistory>();
            _rateCurrent = rateCurrent;
            _eventBus = eventBus;

            LoadRatesFromFile(_filePath);
        }

        public bool CheckPercentageChange()
        {
            if (_rateCurrent.Count > 0 && _rateHistory.Count > 0)
            {
                foreach(var symbol in _rateHistory.Keys)
                {
                    if(_rateCurrent.ContainsKey(symbol))
                    {
                        decimal oldPrice = _rateHistory[symbol].CurrentRate;
                        decimal newPrice = _rateCurrent[symbol].CurrentRate;

                        if (oldPrice > 0) // Prevent division by zero
                        {
                            decimal percentageChange = ((newPrice - oldPrice) / oldPrice) * 100;

                            if (Math.Abs(percentageChange) >= 5) // Change greater than 5%?
                            {
                                string message = $"Stock: {symbol}, Old Price: {oldPrice}, New Price: {newPrice}, Change: {percentageChange:F2}%";
                                Console.WriteLine($"Sending to RabbitMQ: {message}");
                                _eventBus.PublishRateChange(symbol, newPrice);
                            }

                            UpdateRates(symbol, newPrice, _rateCurrent[symbol].LastUpdated);
                        }
                    }
                }
                return true;
            }
            else
            {
                SaveRatesToFile(_filePath, true);
                return false;
            }
        }

        public void UpdateRates(string symbol, decimal newPrice, DateTime LastUpdated)
        {
            // Update the rate history for each cryptocurrency symbol
            if (_rateHistory.ContainsKey(symbol))
            {
                _rateHistory[symbol].CurrentRate = newPrice;
                _rateHistory[symbol].LastUpdated = LastUpdated;
            }
            else
            {
                _rateHistory[symbol] = new RateHistory
                {
                    CurrentRate = newPrice,
                    LastUpdated = LastUpdated
                };
            }

            SaveRatesToFile(_filePath, false);
        }

        public void SaveRatesToFile(string filePath, bool bIsFileEmpty)
        {
            try
            {
                var json = bIsFileEmpty ? JsonSerializer.Serialize(_rateCurrent) : JsonSerializer.Serialize(_rateHistory);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving rates to file: {ex.Message}");
            }
        }

        private void LoadRatesFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    _rateHistory.Clear();
                    var loadedRates = JsonSerializer.Deserialize<Dictionary<string, RateHistory>>(json);

                    if (loadedRates != null)
                    {
                        foreach (var rate in loadedRates)
                        {
                            _rateHistory[rate.Key] = rate.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading rates from file: {ex.Message}");
            }
        }
    }
}

public class RateHistory
{
    public decimal CurrentRate { get; set; }
    public DateTime LastUpdated { get; set; }
}