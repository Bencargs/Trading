using Contracts.Models;
using Contracts.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Market.Providers
{
    public class OrderProvider : IOrderProvider
    {
        private List<Order> _sellOrders = new List<Order>();

        public Order AddBuyOrder(User user, Stock stock, int quantity)
        {
            throw new NotImplementedException();
        }

        public Order AddSellOrder(User user, Stock stock, int quantity)
        {
            var order = new Order
            {
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

        public Order[] GetSellOrders(Stock stock, int quantity)
        {
            int sumQuantity = 0;
            var returnOrders = new List<Order>();
            var sellOrders = _sellOrders.Where(x => x.Stock == stock).OrderBy(x => x.Price).ToArray();
            for (int i = 0; i < sellOrders.Count() && sumQuantity <= quantity; i++)
            {
                sumQuantity += sellOrders[i].Quantity;
                returnOrders.Add(sellOrders[i]);
            }
            return returnOrders.ToArray();
        }

        public void RemoveOrders(Order[] orders)
        {
            throw new NotImplementedException();
        }
    }
}