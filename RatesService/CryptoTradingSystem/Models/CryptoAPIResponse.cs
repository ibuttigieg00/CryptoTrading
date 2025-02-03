namespace RatesService.Models
{
    public class CryptoApiResponse
    {
        public List<CryptoCurrencyData> Data { get; set; }
    }

    public class CryptoCurrencyData
    {
        public string Symbol { get; set; }
        public Quote Quote { get; set; }
    }

    public class Quote
    {
        public CurrencyData USD { get; set; }
    }

    public class CurrencyData
    {
        public decimal Price { get; set; }
        public DateTime Last_Updated { get; set; }
    }
}
