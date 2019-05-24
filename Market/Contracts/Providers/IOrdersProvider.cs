using Contracts.Models;

namespace Contracts.Providers
{
    public interface IOrderProvider
    {
        Order[] GetSellOrders(Stock stock);
        Order[] GetBuyOrders(Stock stock);
        Order AddBuyOrder(User user, Stock stock, decimal price);
        Order AddSellOrder(User user, Stock stock, int quantity, decimal? price = null);
        void UpdateOrders(FillDetail[] fills);
    }
}