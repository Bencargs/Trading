using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contracts.Providers
{
    public interface IOrdersProvider
    {
        Order[] GetSellOrders(Stock stock, int count);
        Order AddBuyOrder(User user, Stock stock, int count);
        void RemoveOrders(Order[] orders);
    }
}