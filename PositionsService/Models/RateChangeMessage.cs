using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionsService.Models
{
    public class RateChangeMessage
    {
        public string Symbol { get; set; }  // e.g., "BTC/USD"
        public decimal NewRate { get; set; } // The updated exchange rate
    }

}
