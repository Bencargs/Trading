using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contracts.Providers
{
    public interface IBankingProvider
    {
        BankAccount GetAccount(User user);

        /// <summary>
        /// Move cost of each order from account into order holders account
        /// </summary>
        /// <param name="user"></param>
        /// <param name="orders"></param>
        void TransferFundsToHolders(BankAccount account, Order[] orders);
    }
}