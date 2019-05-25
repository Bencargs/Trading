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

        public IResponse MarketBuy(User user, Stock stock, int quantity)
        {
            var fills = GetOrderFills(OrderDirection.Buy, stock, quantity);
            if (fills.Any())
            {
                var account = _bankingProvider.GetAccount(user);
                var cost = fills.Sum(x => x.Price * x.Quantity);
                if (account.Balance < cost)
                    return new BuyOrderFailedResponse
                    {
                        Reason = BuyOrderFailedResponse.FailureReason.InsufficientFunds
                    };

                _holdingsProvider.TransferHoldingsToUser(user, fills);
                _bankingProvider.TransferFundsToHolders(user, fills);
                _ordersProvider.UpdateOrders(fills);

                //var minPrice = fills.Max(x => x.Price);
                //_sharesProvider.UpdateLastPrice(stock, minPrice);
            }

            var unfilled = quantity - fills.Sum(x => x.Quantity);
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

        public IResponse MarketSell(User user, Stock stock, int quantity)
        {
            var holdings = _holdingsProvider.GetHolding(user, stock);
            if (holdings < quantity)
                return new SellOrderFailedResponse
                {
                    Reason = SellOrderFailedResponse.FailureReason.InsufficientHoldings
                };

            var fills = GetOrderFills(OrderDirection.Sell, stock, quantity);
            if (fills.Any())
            {
                _holdingsProvider.TransferHoldingsFromUser(user, fills);
                _bankingProvider.TransferFundsFromHolders(user, fills);
                _ordersProvider.UpdateOrders(fills);

            //    var minPrice = fills.Min(x => x.Price);
            //    _sharesProvider.UpdateLastPrice(stock, minPrice);
            }

            var unfilled = quantity - fills.Sum(x => x.Quantity);
            if (unfilled > 0)
            {
                _ordersProvider.AddSellOrder(user, stock, unfilled);
            }

            return new BuyOrderResponse
            {
                Filled = fills,
                Unfilled = unfilled
            };
        }

        private FillDetail[] GetOrderFills(OrderDirection direction, Stock stock, int quantity)
        {
            //var cost = 0m;
            var totalFilled = 0;
            var filledOrders = new List<FillDetail>();

            var orders = direction == OrderDirection.Buy
                ? _ordersProvider.GetSellOrders(stock)
                : _ordersProvider.GetBuyOrders(stock);

            foreach (var o in orders)
            {
                var fill = new FillDetail
                {
                    OrderId = o.OrderId,
                    Owner = o.Owner,
                    Stock = o.Stock,
                    Quantity = 0,
                    Price = o.Price ?? _sharesProvider.GetLastPrice(stock)
                };
                filledOrders.Add(fill);

                for (int i = 0; i < o.Quantity && totalFilled < quantity; i++)
                {
                    //if (totalFilled >= quantity)
                    //    return filledOrders.ToArray();

                    fill.Quantity++;
                    totalFilled++;
                }
            }
            return filledOrders.ToArray();
        }
    }
}