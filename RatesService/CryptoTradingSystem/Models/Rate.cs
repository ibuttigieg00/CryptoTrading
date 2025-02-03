namespace RatesService.Models
{
    public class Rate
    {
        public string Symbol { get; set; } // BTC OR ETH
        public decimal CurrentRate { get; set; }  // Latest rate in USD
        public DateTime Last_Updated { get; set; }  // Last updated timestamp
    }
}