using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using RatesService.EventBus;
using RatesService.Models;
using System.Net;

namespace RatesService.Services
{
    public class RateFetcherService
    {
        private readonly HttpClient _httpClient;
        private readonly IEventBus _eventBus;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly string _filePath;

        public RateFetcherService(HttpClient httpClient, IConfiguration configuration, IEventBus eventBus)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"]!;
            _apiKey = configuration["ApiSettings:ApiKey"]!;
            _filePath = configuration["FileSettings:RateHistoryFilePath"]!;
            _eventBus = eventBus;
        }

        public async Task<HttpResponseMessage> GetRatesAsync()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl);
                request.Headers.Add("X-CMC_PRO_API_KEY", _apiKey); // Replace with actual header name

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    RateChecker _rateChecker = new RateChecker(_filePath, DeserializeToDict(jsonString), _eventBus);
                    bool bisPercentageChanged = _rateChecker.CheckPercentageChange();

                    var message = bisPercentageChanged
                               ? "Percentage change has been detected."
                               : "No significant percentage change detected.";

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(message)
                    };
                }
                else
                {
                    throw new HttpRequestException($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex) 
            {
                string errorMessage = "Something went wrong in RateFetcherService when communicating with CMC api.";
                throw new ArgumentException(errorMessage, ex);
            }
        }

        public Dictionary<string, RateHistory> DeserializeToDict(string jsonString)
        {
            var result = JsonConvert.DeserializeObject<CryptoApiResponse>(jsonString);

            var rates = result.Data.ToDictionary(
                d => d.Symbol, // Key selector
                d => new RateHistory
                { 
                    CurrentRate = d.Quote.USD.Price,
                    LastUpdated = d.Quote.USD.Last_Updated
                }
            );

            return rates;
        }
    }
}