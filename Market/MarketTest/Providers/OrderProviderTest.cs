using System;
using System.Linq;
using Contracts.Models;
using Contracts.Providers;
using Market.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarketTest.Providers
{
    [TestClass]
    public class OrderProviderTest
    {
        private readonly IOrderProvider _provider;
        private readonly Stock _stock;
        private readonly User _user;

        public OrderProviderTest()
        {
            _provider = new OrderProvider();
            _stock = new Stock
            {
                Ticker = "ABC"
            };
            _user = new User
            {
                UserId = Guid.NewGuid(),
                Username = "TestUser"
            };
        }

        [TestMethod]
        public void GetSellOrdersTest_OrderCount()
        {
            var quantity = 10;
            _provider.AddSellOrder(_user, _stock, quantity);

            var actual = _provider.GetSellOrders(_stock, quantity);

            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void GetSellOrdersTest_CorrectStock()
        {
            var quantity = 10;
            _provider.AddSellOrder(_user, _stock, quantity);

            var actual = _provider.GetSellOrders(_stock, quantity);

            Assert.AreEqual(_stock, actual[0].Stock);
        }

        [TestMethod]
        public void GetSellOrdersTest_CorrectCount()
        {
            var quantity = 10;
            _provider.AddSellOrder(_user, _stock, quantity);

            var actual = _provider.GetSellOrders(_stock, quantity);

            Assert.AreEqual(quantity, actual[0].Quantity);
        }

        [TestMethod]
        public void GetSellOrdersTest_CorrectDirection()
        {
            var quantity = 10;
            _provider.AddSellOrder(_user, _stock, quantity);

            var actual = _provider.GetSellOrders(_stock, quantity);

            Assert.AreEqual(OrderDirection.Sell, actual[0].Direction);
        }
    }
}
