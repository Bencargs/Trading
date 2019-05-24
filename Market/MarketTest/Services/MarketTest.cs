using Contracts.Models;
using Contracts.Providers;
using Contracts.Responses;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MarketTest.Services
{
    [TestClass]
    public class MarketTest : TestHarness
    {
        private readonly Stock _stock = new Stock { Ticker = "ABC" };
        private readonly User _user = new User { UserId = Guid.NewGuid(), Username = "TestUser" };
        
        [TestMethod]
        public void MarketBuyTest_CorrectFundsAllocated()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 1000m);
            var seller = new User { UserId = Guid.NewGuid() };
            RegisterUser(seller, 0m,
                holdings: new Dictionary<Stock, int>
                {
                    { _stock, 20 }
                },
                sellOrders: new Dictionary<Stock, OrderProxy>
                {
                    { _stock, new OrderProxy { Quantity = 20, Price = 10m } }
                });

            Market.MarketBuy(_user, _stock, 100m);

            Assert.AreEqual(900m, Resolve<IBankingProvider>().GetAccount(_user).Balance);
            Assert.AreEqual(100m, Resolve<IBankingProvider>().GetAccount(seller).Balance);
        }

        [TestMethod]
        public void MarketBuyTest_CorrectHoldingsAllocated()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 1000m);
            var seller = new User { UserId = Guid.NewGuid() };
            RegisterUser(seller, 0m,
                holdings: new Dictionary<Stock, int>
                {
                    { _stock, 20 }
                },
                sellOrders: new Dictionary<Stock, OrderProxy>
                {
                    { _stock, new OrderProxy { Quantity = 20, Price = 10m } }
                });

            Market.MarketBuy(_user, _stock, 100m);

            Assert.AreEqual(10, Resolve<IHoldingsProvider>().GetHolding(_user, _stock));
            Assert.AreEqual(10, Resolve<IHoldingsProvider>().GetHolding(seller, _stock));
        }

        [TestMethod]
        public void MarketBuyTest_WhereNoSellPriceSet()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 1000m);
            var seller = new User { UserId = Guid.NewGuid() };
            RegisterUser(seller, 0m,
                holdings: new Dictionary<Stock, int>
                {
                    { _stock, 20 }
                },
                sellOrders: new Dictionary<Stock, OrderProxy>
                {
                    { _stock, new OrderProxy { Quantity = 20, Price = null } }
                });

            Market.MarketBuy(_user, _stock, 100m);

            Assert.AreEqual(900m, Resolve<IBankingProvider>().GetAccount(_user).Balance);
        }

        [TestMethod]
        public void MarketBuyTest_WhereUnfilledBuy()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 1000m);
            var seller = new User { UserId = Guid.NewGuid() };
            RegisterUser(seller, 0m,
                holdings: new Dictionary<Stock, int>
                {
                    { _stock, 5 }
                },
                sellOrders: new Dictionary<Stock, OrderProxy>
                {
                    { _stock, new OrderProxy { Quantity = 5 } }
                });

            Market.MarketBuy(_user, _stock, 100m);

            var expected = Resolve<IOrderProvider>().GetBuyOrders(_stock);
            expected.Should().BeEquivalentTo(
                new
                {
                    Direction = OrderDirection.Buy,
                    Type = OrderType.Market,
                    Owner = _user,
                    Price = 50m,
                    Stock = _stock
                });
        }

        [TestMethod]
        public void MarketBuyTest_InsufficentFunds()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 10m);

            var response = Market.MarketBuy(_user, _stock, 100m);

            Assert.AreEqual(
                BuyOrderFailedResponse.FailureReason.InsufficientFunds,
                ((BuyOrderFailedResponse)response).Reason);
        }

        [TestMethod]
        public void MarketBuyTest_NoSellers()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 100m);

            var response = Market.MarketBuy(_user, _stock, 100m);

            response.Should().BeEquivalentTo(
                new BuyOrderResponse
                {
                    Filled = new FillDetail[0],
                    Unfilled = 100m
                });
        }
    }
}
