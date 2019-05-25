using Contracts.Models;
using Contracts.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Exchange.Providers
{
    public class OrderProvider : IOrderProvider
    {
        private Dictionary<Stock, List<Order>> _sellOrders = new Dictionary<Stock, List<Order>>();
        private Dictionary<Stock, List<Order>> _buyOrders = new Dictionary<Stock, List<Order>>();

        public Order AddBuyOrder(User user, Stock stock, int quantity)
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                Owner = user,
                Stock = stock,
                Direction = OrderDirection.Buy,
                Type = OrderType.Market,
                Quantity = quantity,
                Price = null
            };

            if (_buyOrders.ContainsKey(stock))
                _buyOrders[stock].Add(order);
            else
                _buyOrders.Add(stock, new List<Order> { order });

            return order;
        }

        public Order AddSellOrder(User user, Stock stock, int quantity)
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                Owner = user,
                Stock = stock,
                Direction = OrderDirection.Sell,
                Type = OrderType.Market,
                Quantity = quantity,
                Price = null
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
                ? orders.OrderByDescending(x => x.Price).ToArray()
                : new Order[0];
        }

        public Order[] GetSellOrders(Stock stock)
        {
            return _sellOrders.TryGetValue(stock, out List<Order> orders)
                ? orders.OrderBy(x => x.Price).ToArray()
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