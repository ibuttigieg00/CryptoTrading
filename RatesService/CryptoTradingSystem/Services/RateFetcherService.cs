namespace RatesService.Services
{
    public class RateFetcherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public  RateFetcherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"]!;
            _apiKey = configuration["ApiSettings:ApiKey"]!;
        }

        public async Task<string> GetRatesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl);
            request.Headers.Add("X-CMC_PRO_API_KEY", _apiKey); // Replace with actual header name

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new HttpRequestException($"Error: {response.StatusCode}");
            }
        }
    }
}
