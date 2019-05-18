using System;

namespace Contracts.Models
{
    public class FillDetail
    {
        public User Owner { get; set; }
        public Guid OrderId { get; set; }
        public Stock Stock { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
