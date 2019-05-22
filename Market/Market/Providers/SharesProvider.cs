using Contracts.Models;
using Contracts.Providers;
using System.Collections.Generic;

namespace Market.Providers
{
    public class SharesProvider : ISharesProvider
    {
        private Dictionary<Stock, decimal> _shares = new Dictionary<Stock, decimal>();

        public void AddStock(Stock stock, decimal price)
        {
            _shares.Add(stock, price);
        }

        public decimal GetLastPrice(Stock stock)
        {
            return _shares[stock];
        }

        public void UpdateLastPrice(Stock stock, decimal price)
        {
            _shares[stock] = price;
        }
    }
}