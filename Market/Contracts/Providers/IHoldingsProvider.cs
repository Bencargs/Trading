using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Providers
{
    public interface IHoldingsProvider
    {
        void CreateUser(User user);
        void AddHolding(User user, Stock stock, int quantity);
        int GetHolding(User user, Stock stock);

        /// <summary>
        /// Move all holdings in orders to users name
        /// </summary>
        /// <param name="user"></param>
        /// <param name="orders"></param>
        void TransferHoldingsToUser(User user, FillDetail[] fills);
    }
}
