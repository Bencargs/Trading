using System;

namespace Contracts.Models
{
    public class Trade
    {
        public DateTime Timestamp { get; set; }
        public int Volume { get; set; }
        public decimal Price { get; set; }
    }
}
