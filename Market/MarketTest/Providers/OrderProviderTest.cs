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
            _provider.AddSellOrder(_user, _stock, 1);
            _provider.AddSellOrder(_user, _stock, 1);

            var actual = _provider.GetSellOrders(_stock);

            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void GetSellOrdersTest_CorrectStock()
        {
            _provider.AddSellOrder(_user, _stock, 1);
            _provider.AddSellOrder(_user, new Stock { Ticker = "XYZ" }, 1);

            var actual = _provider.GetSellOrders(_stock);

            Assert.IsTrue(actual.All(x => x.Stock == _stock));
        }

        [TestMethod]
        public void GetSellOrdersTest_CorrectDirection()
        {
            _provider.AddSellOrder(_user, _stock, 1);
            _provider.AddBuyOrder(_user, _stock, 1);

            var actual = _provider.GetSellOrders(_stock);

            Assert.IsTrue(actual.All(x => x.Direction == OrderDirection.Sell));
        }

        [TestMethod]
        public void UpdateOrdersTest_Sell()
        {
            _provider.AddSellOrder(_user, _stock, 10);
            var order = _provider.GetSellOrders(_stock);
            _provider.UpdateOrders(new[]
            {
                new FillDetail
                {
                    OrderId = order[0].OrderId,
                    Stock = _stock,
                    Quantity = 7,
                }
            });

            var actual = _provider.GetSellOrders(_stock);

            Assert.AreEqual(3, actual.Sum(x => x.Quantity));
        }

        [TestMethod]
        public void UpdateOrdersTest_SellNoQuantity()
        {
            _provider.AddSellOrder(_user, _stock, 10);
            var order = _provider.GetSellOrders(_stock);
            _provider.UpdateOrders(new[]
            {
                new FillDetail
                {
                    OrderId = order[0].OrderId,
                    Stock = _stock,
                    Quantity = 0,
                }
            });

            var actual = _provider.GetSellOrders(_stock);

            Assert.AreEqual(10, actual.Sum(x => x.Quantity));
        }

        [TestMethod]
        public void UpdateOrdersTest_SellOrderFilled()
        {
            _provider.AddSellOrder(_user, _stock, 10);
            var order = _provider.GetSellOrders(_stock);
            _provider.UpdateOrders(new[]
            {
                new FillDetail
                {
                    OrderId = order[0].OrderId,
                    Stock = _stock,
                    Quantity = 10,
                }
            });

            var actual = _provider.GetSellOrders(_stock);

            Assert.AreEqual(0, actual.Sum(x => x.Quantity));
        }
    }
}
