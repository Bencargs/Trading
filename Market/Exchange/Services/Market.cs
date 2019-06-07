using Contracts;
using Contracts.Models;
using Contracts.Providers;
using Contracts.Responses;
using System.Collections.Generic;
using System.Linq;

namespace Exchange.Services
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
            var fills = GetOrderFills(OrderType.Market, OrderDirection.Buy, stock, quantity);
            if (fills.Any())
            {
                var account = _bankingProvider.GetAccount(user);
                var cost = fills.Sum(x => x.Price * x.Quantity);
                if (account.Balance < cost)
                    return new BuyOrderFailedResponse
                    { Reason = BuyOrderFailedResponse.FailureReason.InsufficientFunds };

                ExecuteBuy(user, fills);
            }

            var unfilled = quantity - fills.Sum(x => x.Quantity);
            if (unfilled > 0)
                _ordersProvider.AddBuyOrder(user, stock, unfilled);

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
                { Reason = SellOrderFailedResponse.FailureReason.InsufficientHoldings };

            var fills = GetOrderFills(OrderType.Market, OrderDirection.Sell, stock, quantity);
            if (fills.Any())
                ExecuteSell(user, fills);

            var unfilled = quantity - fills.Sum(x => x.Quantity);
            if (unfilled > 0)
                _ordersProvider.AddSellOrder(user, stock, unfilled);

            return new BuyOrderResponse
            {
                Filled = fills,
                Unfilled = unfilled
            };
        }

        public IResponse LimitBuy(User user, Stock stock, int quantity, decimal price)
        {
            var cost = quantity * price;
            var account = _bankingProvider.GetAccount(user);
            if (account.Balance < cost)
                return new BuyOrderFailedResponse
                {
                    Reason = BuyOrderFailedResponse.FailureReason.InsufficientFunds
                };

            var fills = GetOrderFills(OrderType.Limit, OrderDirection.Buy, stock, quantity, price);
            if (fills.Any())
                ExecuteBuy(user, fills);

            var unfilled = quantity - fills.Sum(x => x.Quantity);
            if (unfilled > 0)
                _ordersProvider.AddBuyOrder(user, stock, unfilled, price);

            return new BuyOrderResponse
            {
                Filled = fills,
                Unfilled = unfilled
            };
        }

        public IResponse LimitSell(User user, Stock stock, int quantity, decimal price)
        {
            var holdings = _holdingsProvider.GetHolding(user, stock);
            if (holdings < quantity)
                return new SellOrderFailedResponse
                { Reason = SellOrderFailedResponse.FailureReason.InsufficientHoldings };

            var fills = GetOrderFills(OrderType.Limit, OrderDirection.Sell, stock, quantity, price);
            if (fills.Any())
                ExecuteSell(user, fills);

            var unfilled = quantity - fills.Sum(x => x.Quantity);
            if (unfilled > 0)
                _ordersProvider.AddSellOrder(user, stock, unfilled, price);

            return new BuyOrderResponse
            {
                Filled = fills,
                Unfilled = unfilled
            };
        }

        private void ExecuteBuy(User user, FillDetail[] fills)
        {
            _holdingsProvider.TransferHoldingsToUser(user, fills);
            _bankingProvider.TransferFundsToHolders(user, fills);
            _ordersProvider.UpdateOrders(fills);
            _sharesProvider.UpdateStock(fills);
        }

        private void ExecuteSell(User user, FillDetail[] fills)
        {
            _holdingsProvider.TransferHoldingsFromUser(user, fills);
            _bankingProvider.TransferFundsFromHolders(user, fills);
            _ordersProvider.UpdateOrders(fills);
            _sharesProvider.UpdateStock(fills);
        }

        private FillDetail[] GetOrderFills(
            OrderType type, 
            OrderDirection direction, 
            Stock stock, 
            int quantity, 
            decimal? price = null)
        {
            var totalFilled = 0;
            var filledOrders = new List<FillDetail>();

            var orders = type == OrderType.Market
                ? GetMarketOrders(direction, stock)
                : GetLimitOrders(direction, stock, price.Value);

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

                for (int i = 0; i < o.Quantity; i++)
                {
                    if (totalFilled >= quantity)
                        return filledOrders.ToArray();

                    fill.Quantity++;
                    totalFilled++;
                }
            }
            return filledOrders.ToArray();
        }

        private Order[] GetMarketOrders(OrderDirection direction, Stock stock)
        {
            return direction == OrderDirection.Buy
                ? _ordersProvider.GetSellOrders(stock)
                : _ordersProvider.GetBuyOrders(stock);
        }

        private Order[] GetLimitOrders(OrderDirection direction, Stock stock, decimal price)
        {
            return direction == OrderDirection.Buy
                ? _ordersProvider.GetSellOrders(stock).Where(x => (x.Price ?? _sharesProvider.GetLastPrice(stock)) <= price).ToArray()
                : _ordersProvider.GetBuyOrders(stock).Where(x => (x.Price ?? _sharesProvider.GetLastPrice(stock)) >= price).ToArray();
        }
    }
}