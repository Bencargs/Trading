using Contracts.Models;

namespace Contracts.Providers
{
    public interface ISharesProvider
    {
        void AddStock(Stock stock, decimal price);
        decimal GetLastPrice(Stock stock);
        Trade[] GetTrades(Stock stock);
        void UpdateStock(FillDetail[] fills);
    }
}
