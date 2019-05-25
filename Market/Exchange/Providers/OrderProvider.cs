using Contracts.Models;
using Contracts.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Exchange.Providers
{
    public class OrderProvider : IOrderProvider
    {
        private List<Order> _sellOrders = new List<Order>();
        private List<Order> _buyOrders = new List<Order>();

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
            _buyOrders.Add(order);
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
            _sellOrders.Add(order);
            return order;
        }

        public Order[] GetBuyOrders(Stock stock)
        {
            return _buyOrders
                .Where(x => x.Stock == stock)
                .OrderByDescending(x => x.Price)
                .ToArray();
        }

        public Order[] GetSellOrders(Stock stock)
        {
            return _sellOrders
                .Where(x => x.Stock == stock)
                .OrderBy(x => x.Price)
                .ToArray();
        }

        public void UpdateOrders(FillDetail[] fills)
        {
            foreach (var f in fills)
            {
                if (f.Quantity == 0)
                    continue;

                var order = _sellOrders.SingleOrDefault(x => x.OrderId == f.OrderId);
                if (order != null)
                {
                    if (f.Quantity == order.Quantity)
                    {
                        _sellOrders.Remove(order);
                    }
                    else
                    {
                        order.Quantity -= f.Quantity;
                    }
                }
                else
                {
                    order = _buyOrders.Single(x => x.OrderId == f.OrderId);
                    if (f.Quantity == order.Quantity)
                    {
                        _buyOrders.Remove(order);
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