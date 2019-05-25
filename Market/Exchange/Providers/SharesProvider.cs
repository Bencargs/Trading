using Contracts.Models;
using Contracts.Providers;
using System.Collections.Generic;
using System.Linq;

namespace Exchange.Providers
{
    public class SharesProvider : ISharesProvider
    {
        private Dictionary<Stock, decimal> _shares = new Dictionary<Stock, decimal>();
        private Dictionary<Stock, List<Trade>> _trades = new Dictionary<Stock, List<Trade>>();

        public void AddStock(Stock stock, decimal price)
        {
            _shares.Add(stock, price);
            _trades[stock] = new List<Trade>();
        }

        public decimal GetLastPrice(Stock stock)
        {
            return _shares[stock];
        }

        public Trade[] GetTrades(Stock stock)
        {
            return _trades[stock].ToArray();
        }

        public void UpdateStock(FillDetail[] fills)
        {
            foreach (var f in fills.GroupBy(x => x.Stock))
            {
                var price = f.Last().Price;
                var volume = f.Sum(x => x.Quantity);

                _shares[f.Key] = price;
                _trades[f.Key].Add(new Trade
                {
                    Price = price,
                    Volume = volume
                });
            }
        }
    }
}