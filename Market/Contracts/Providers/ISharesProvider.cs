using Contracts.Models;

namespace Contracts.Providers
{
    public interface ISharesProvider
    {
        decimal GetLastPrice(Stock stock);
        void UpdateLastPrice(Stock stock, decimal price);
    }
}
