﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Responses
{
    public class SellOrderFailedResponse : IResponse
    {
        public FailureReason Reason { get; set; }

        public enum FailureReason
        {
            None = 0,
            OrderDirection,
            InsufficientHoldings
        }
    }
}
