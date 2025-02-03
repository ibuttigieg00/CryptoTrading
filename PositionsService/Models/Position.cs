namespace PositionsService.Models
{
    public class Position
    {
        public string InstrumentId { get; set; } // e.g., "BTC/USD"
        public decimal Quantity { get; set; } // The amount of base currency
        public decimal InitialRate { get; set; } // The rate at which the position was opened
        public string Side { get; set; } // "BUY" or "SELL"
    }
}