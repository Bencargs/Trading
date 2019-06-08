using Contracts;
using Contracts.Models;
using Contracts.Providers;
using System;
using System.Collections.Generic;

namespace Exchange.Providers
{
    public class HoldingsProvider : IHoldingsProvider
    {
        private Dictionary<User, Dictionary<Stock, int>> _holdings = new Dictionary<User, Dictionary<Stock, int>>();

        public void CreateUser(User user)
        {
            _holdings.Add(user, new Dictionary<Stock, int>());
        }

        public void AddHolding(User user, Stock stock, int quantity)
        {
            _holdings[user].Add(stock, quantity);
        }

        public Dictionary<Stock, int> GetHoldings(User user)
        {
            return _holdings[user];
        }

        public int GetHolding(User user, Stock stock)
        {
            _holdings[user].TryGetValue(stock, out int stocksHeld);
            return stocksHeld;
        }

        public void TransferHoldingsToUser(User user, FillDetail[] fills)
        {
            var userHoldings = _holdings[user];
            foreach (var f in fills)
            {
                var fillHoldings = _holdings[f.Owner];

                fillHoldings[f.Stock] -= f.Quantity;

                if (userHoldings.ContainsKey(f.Stock))
                    userHoldings[f.Stock] += f.Quantity;
                else
                    userHoldings.Add(f.Stock, f.Quantity);
            }
        }

        public void TransferHoldingsFromUser(User user, FillDetail[] fills)
        {
            var userHoldings = _holdings[user];
            foreach (var f in fills)
            {
                userHoldings[f.Stock] -= f.Quantity;

                var fillHoldings = _holdings[f.Owner];

                if (fillHoldings.ContainsKey(f.Stock))
                    fillHoldings[f.Stock] += f.Quantity;
                else
                    fillHoldings.Add(f.Stock, f.Quantity);
            }
        }
    }
}