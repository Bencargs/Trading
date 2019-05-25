﻿using Contracts.Models;

namespace Contracts.Responses
{
    public class BuyOrderResponse : IResponse
    {
        public FillDetail[] Filled { get; set; }
        public int Unfilled { get; set; }
    }
}
