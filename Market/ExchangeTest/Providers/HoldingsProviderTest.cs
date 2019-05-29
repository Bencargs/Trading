using Contracts.Models;
using Contracts.Providers;
using Exchange.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ExchangeTests.Providers
{
    [TestClass]
    public class HoldingsProviderTest
    {
        private readonly IHoldingsProvider _provider;
        private readonly Stock _stock;
        private readonly User _user;

        public HoldingsProviderTest()
        {
            _provider = new HoldingsProvider();
            _user = new User { UserId = Guid.NewGuid() };
            _stock = new Stock { Ticker = "ABC" };
        }

        [TestMethod]
        public void GetHoldingsTest()
        {
            _provider.CreateUser(_user);

            var actual = _provider.GetHolding(_user, _stock);

            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void AddHoldingTest()
        {
            _provider.CreateUser(_user);
            _provider.AddHolding(_user, _stock, 4);

            var actual = _provider.GetHolding(_user, _stock);

            Assert.AreEqual(4, actual);
        }

        [TestMethod]
        public void TransferHoldingsToUserTest()
        {
            var fills = new[]
            {
                new FillDetail
                {
                    Owner = new User { UserId = Guid.NewGuid()},
                    Stock = _stock,
                    Quantity = 5
                },
                new FillDetail
                {
                    Owner = new User { UserId = Guid.NewGuid()},
                    Stock = _stock,
                    Quantity = 7
                },
            };
            _provider.CreateUser(_user);
            _provider.AddHolding(_user, _stock, 3);
            _provider.CreateUser(fills[0].Owner);
            _provider.AddHolding(fills[0].Owner, _stock, 5);
            _provider.CreateUser(fills[1].Owner);
            _provider.AddHolding(fills[1].Owner, _stock, 7);

            _provider.TransferHoldingsToUser(_user, fills);

            var userHoldings = _provider.GetHolding(_user, _stock);
            var fill1Holdings = _provider.GetHolding(fills[0].Owner, _stock);
            var fill2Holdings = _provider.GetHolding(fills[1].Owner, _stock);
            Assert.AreEqual(15, userHoldings);
            Assert.AreEqual(0, fill1Holdings);
            Assert.AreEqual(0, fill2Holdings);
        }
    }
}
