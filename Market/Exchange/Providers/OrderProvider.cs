using Contracts;
using Contracts.Models;
using Contracts.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Exchange.Providers
{
    public class OrderProvider : IOrderProvider
    {
        private readonly IDateTimeSource _dateTimeSource;
        private Dictionary<Stock, List<Order>> _sellOrders = new Dictionary<Stock, List<Order>>();
        private Dictionary<Stock, List<Order>> _buyOrders = new Dictionary<Stock, List<Order>>();

        public OrderProvider(IDateTimeSource dateTimeSource)
        {
            _dateTimeSource = dateTimeSource;
        }

        public Order AddBuyOrder(User user, Stock stock, int quantity, decimal? price = null)
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                Owner = user,
                Timestamp = _dateTimeSource.Now,
                Stock = stock,
                Direction = OrderDirection.Buy,
                Type = price == null ? OrderType.Market : OrderType.Limit,
                Quantity = quantity,
                Price = price
            };

            if (_buyOrders.ContainsKey(stock))
                _buyOrders[stock].Add(order);
            else
                _buyOrders.Add(stock, new List<Order> { order });

            return order;
        }

        public Order AddSellOrder(User user, Stock stock, int quantity, decimal? price = null)
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                Owner = user,
                Timestamp = _dateTimeSource.Now,
                Stock = stock,
                Direction = OrderDirection.Sell,
                Type = price == null ? OrderType.Market : OrderType.Limit,
                Quantity = quantity,
                Price = price
            };

            if (_sellOrders.ContainsKey(stock))
                _sellOrders[stock].Add(order);
            else
                _sellOrders.Add(stock, new List<Order> { order });

            return order;
        }

        public Order[] GetBuyOrders(Stock stock)
        {
            return _buyOrders.TryGetValue(stock, out List<Order> orders)
                ? orders.OrderByDescending(x => x.Price).ThenBy(x => x.Timestamp).ToArray()
                : new Order[0];
        }

        public Order[] GetSellOrders(Stock stock)
        {
            return _sellOrders.TryGetValue(stock, out List<Order> orders)
                ? orders.OrderBy(x => x.Price).ThenBy(x => x.Timestamp).ToArray()
                : new Order[0];
        }

        public void UpdateOrders(FillDetail[] fills)
        {
            foreach (var f in fills)
            {
                if (f.Quantity == 0)
                    continue;

                var order = GetSellOrders(f.Stock).SingleOrDefault(x => x.OrderId == f.OrderId);
                if (order != null)
                {
                    if (f.Quantity == order.Quantity)
                    {
                        _sellOrders[f.Stock].Remove(order);
                    }
                    else
                    {
                        order.Quantity -= f.Quantity;
                    }
                }
                else
                {
                    order = GetBuyOrders(f.Stock).Single(x => x.OrderId == f.OrderId);
                    if (f.Quantity == order.Quantity)
                    {
                        _buyOrders[f.Stock].Remove(order);
                    }
                    else
                    {
                        order.Quantity -= f.Quantity;
                    }
                }
            }
        }
    }
}