using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contracts.Providers
{
    public interface IOrderProvider
    {
        Order[] GetSellOrders(Stock stock, int quantity);
        Order AddBuyOrder(User user, Stock stock, int quantity);
        Order AddSellOrder(User user, Stock stock, int quantity);
        void RemoveOrders(Order[] orders);
    }
}