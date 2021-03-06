﻿using Contracts;
using Contracts.Models;
using Contracts.Providers;
using System.Collections.Generic;

namespace Exchange.Providers
{
    public class BankingProvider : IBankingProvider
    {
        private Dictionary<User, BankAccount> _accounts = new Dictionary<User, BankAccount>();

        public void CreateAccount(User user)
        {
            _accounts.Add(user, new BankAccount
            {
                Balance = 0
            });
        }

        public void AddFunds(User user, decimal funds)
        {
            var userAccount = _accounts[user];
            userAccount.Balance += funds;
        }

        public BankAccount GetAccount(User user)
        {
            _accounts.TryGetValue(user, out BankAccount account);
            return account;
        }

        public void TransferFundsToHolders(User user, FillDetail[] fills)
        {
            var userAccount = _accounts[user];
            foreach (var f in fills)
            {
                var holderAccount = _accounts[f.Owner];

                var cost = f.Price * f.Quantity;
                userAccount.Balance -= cost;
                holderAccount.Balance += cost;
            }
        }

        public void TransferFundsFromHolders(User user, FillDetail[] fills)
        {
            var userAccount = _accounts[user];
            foreach (var f in fills)
            {
                var holderAccount = _accounts[f.Owner];

                var cost = f.Price * f.Quantity;
                userAccount.Balance += cost;
                holderAccount.Balance -= cost;
            }
        }
    }
}