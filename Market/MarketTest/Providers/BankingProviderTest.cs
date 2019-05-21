using Contracts.Models;
using Contracts.Providers;
using Market.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MarketTest.Providers
{
    [TestClass]
    public class BankingProviderTest
    {
        private readonly IBankingProvider _provider;
        private readonly User _user;

        public BankingProviderTest()
        {
            _provider = new BankingProvider();
            _user = new User
            {
                UserId = Guid.NewGuid(),
                Username = "TestUser"
            };
        }

        [TestMethod]
        public void GetAccountTest()
        {
            _provider.CreateAccount(_user);

            var actual = _provider.GetAccount(_user);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void AddFundsTest()
        {
            _provider.CreateAccount(_user);
            var account = _provider.GetAccount(_user);
            _provider.AddFunds(_user, 100m);

            var actual = _provider.GetAccount(_user);

            Assert.AreEqual(100m, account.Balance);
        }

        [TestMethod]
        public void TransferFundsToHoldersTest()
        {
            var stock = new Stock { Ticker = "ABC" };
            var fills = new[]
            {
                new FillDetail
                {
                    Owner = new User {UserId = Guid.NewGuid()},
                    OrderId = Guid.NewGuid(),
                    Stock = stock,
                    Quantity = 5,
                    Price = 7
                }, // TotalCost = 5 * 7 = 35m
                new FillDetail
                {
                    Owner = new User {UserId = Guid.NewGuid()},
                    OrderId = Guid.NewGuid(),
                    Stock = stock,
                    Quantity = 3,
                    Price = 9
                },// TotalCost = 3 * 9 = 27m
            };
            _provider.CreateAccount(_user);
            _provider.CreateAccount(fills[0].Owner);
            _provider.CreateAccount(fills[1].Owner);
            _provider.AddFunds(_user, 100m);

            var userAccount = _provider.GetAccount(_user);
            _provider.TransferFundsToHolders(_user, fills);

            var finalUserBalance = _provider.GetAccount(_user).Balance;
            var fill1Balance = _provider.GetAccount(fills[0].Owner).Balance;
            var fill2Balance = _provider.GetAccount(fills[1].Owner).Balance;

            Assert.AreEqual(38m, finalUserBalance);
            Assert.AreEqual(35m, fill1Balance);
            Assert.AreEqual(27m, fill2Balance);
        }
    }
}
