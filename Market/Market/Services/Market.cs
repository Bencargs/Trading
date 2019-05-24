using Contracts.Models;
using Contracts.Providers;
using Contracts.Responses;
using System.Collections.Generic;
using System.Linq;

namespace Market.Services
{
    public class Market
    {
        private readonly IOrderProvider _ordersProvider;
        private readonly IHoldingsProvider _holdingsProvider;
        private readonly IBankingProvider _bankingProvider;
        private readonly ISharesProvider _sharesProvider;

        public Market(
            IOrderProvider ordersProvider,
            IHoldingsProvider holdingsProvider,
            IBankingProvider bankingProvider,
            ISharesProvider sharesProvider)
        {
            _ordersProvider = ordersProvider;
            _holdingsProvider = holdingsProvider;
            _bankingProvider = bankingProvider;
            _sharesProvider = sharesProvider;
        }

        public IResponse MarketBuy(User user, Stock stock, decimal funds)
        {
            var account = _bankingProvider.GetAccount(user);
            if (account.Balance < funds)
                return new BuyOrderFailedResponse
                {
                    Reason = BuyOrderFailedResponse.FailureReason.InsufficientFunds
                };

            var fills = GetBuyOrderFills(stock, funds);
            if (fills.Any())
            {
                _holdingsProvider.TransferHoldingsToUser(user, fills);
                _bankingProvider.TransferFundsToHolders(user, fills);
                _ordersProvider.UpdateOrders(fills);

                var minPrice = fills.Max(x => x.Price);
                _sharesProvider.UpdateLastPrice(stock, minPrice);
            }

            var unfilled = funds - fills.Sum(x => x.Price * x.Quantity);
            if (unfilled > 0)
            {
                _ordersProvider.AddBuyOrder(user, stock, unfilled);
            }

            return new BuyOrderResponse
            {
                Filled = fills,
                Unfilled = unfilled
            };
        }

        private FillDetail[] GetBuyOrderFills(Stock stock, decimal funds)
        {
            var cost = 0m;
            var returnOrders = new List<FillDetail>();
            foreach (var o in _ordersProvider.GetSellOrders(stock))
            {
                var fill = new FillDetail
                {
                    OrderId = o.OrderId,
                    Owner = o.Owner,
                    Stock = o.Stock,
                    Quantity = 0,
                    Price = o.Price ?? _sharesProvider.GetLastPrice(stock)
                };
                returnOrders.Add(fill);

                for (int i = 0; i < o.Quantity; i++)
                {
                    if (cost >= funds)
                        return returnOrders.ToArray();

                    fill.Quantity++;
                    cost += fill.Price;
                }
            }
            return returnOrders.ToArray();
        }
    }
}