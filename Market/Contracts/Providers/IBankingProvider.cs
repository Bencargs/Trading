using Contracts.Models;

namespace Contracts.Providers
{
    public interface IBankingProvider
    {
        void CreateAccount(User user);
        void AddFunds(User user, decimal funds);
        BankAccount GetAccount(User user);

        /// <summary>
        /// Move cost of each order from account into order holders account
        /// </summary>
        /// <param name="user"></param>
        /// <param name="orders"></param>
        void TransferFundsToHolders(User user, FillDetail[] fills);
        void TransferFundsFromHolders(User user, FillDetail[] fills);
    }
}