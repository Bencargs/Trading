using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Responses
{
    public class BuyOrderResponse : IResponse
    {
        public Order[] Filled { get; set; }
        public Order Unfilled { get; set; }
    }
}
