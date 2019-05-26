using System;

namespace Contracts.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public User Owner { get; set; }
        public DateTime Timestamp { get; set; }
        public Stock Stock { get; set; }
        public OrderDirection Direction { get; set; }
        public OrderType Type { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}