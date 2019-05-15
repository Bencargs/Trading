using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contracts.Models
{
    public class Order
    {
        public Stock Stock { get; set; }
        public OrderDirection Direction { get; set; }
        public OrderType Type { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }

        public enum OrderDirection
        {
            None = 0,
            Buy,
            Sell
        }
    }
}