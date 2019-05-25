using Contracts.Models;

namespace Contracts.Responses
{
    public class SellOrderResponse : IResponse
    {
        public FillDetail[] Filled { get; set; }
        public int Unfilled { get; set; }
    }
}
