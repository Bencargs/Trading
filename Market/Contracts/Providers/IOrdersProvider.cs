using Contracts.Models;

namespace Contracts.Providers
{
    public interface IOrderProvider
    {
        Order[] GetSellOrders(Stock stock);
        Order[] GetBuyOrders(Stock stock);
        Order AddBuyOrder(User user, Stock stock, int quantity);
        Order AddSellOrder(User user, Stock stock, int quantity);
        void UpdateOrders(FillDetail[] fills);
    }
}