using Contracts;
using Contracts.Models;
using Contracts.Providers;
using Contracts.Responses;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeTests.Services
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
                sellOrders: new Dictionary<Stock, int>
                {
                    { _stock, 10 }
                });

            Market.MarketBuy(_user, _stock, 100);

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
                sellOrders: new Dictionary<Stock, int>
                {
                    { _stock, 10 }
                });

            Market.MarketBuy(_user, _stock, 100);

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
                sellOrders: new Dictionary<Stock, int>
                {
                    { _stock, 20 }
                });

            Market.MarketBuy(_user, _stock, 100);

            Assert.AreEqual(800m, Resolve<IBankingProvider>().GetAccount(_user).Balance);
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
                sellOrders: new Dictionary<Stock, int>
                {
                    { _stock, 5 }
                });

            Market.MarketBuy(_user, _stock, 100);

            var expected = Resolve<IOrderProvider>().GetBuyOrders(_stock);
            expected.Should().BeEquivalentTo(
                new
                {
                    Direction = OrderDirection.Buy,
                    Type = OrderType.Market,
                    Owner = _user,
                    Stock = _stock,
                    Quantity = 95,
                });
        }

        [TestMethod]
        public void MarketBuyTest_InsufficentFunds()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 10m);
            RegisterUser(new User { UserId = Guid.NewGuid() }, 0m,
                holdings: new Dictionary<Stock, int>
                {
                    { _stock, 100 }
                },
                sellOrders: new Dictionary<Stock, int>
                {
                    { _stock, 100 }
                });

            var response = Market.MarketBuy(_user, _stock, 100);

            Assert.AreEqual(
                BuyOrderFailedResponse.FailureReason.InsufficientFunds,
                ((BuyOrderFailedResponse)response).Reason);
        }

        [TestMethod]
        public void MarketBuyTest_NoSellers()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 100m);

            var response = Market.MarketBuy(_user, _stock, 10);

            response.Should().BeEquivalentTo(
                new BuyOrderResponse
                {
                    Filled = new FillDetail[0],
                    Unfilled = 10
                });
        }

        [TestMethod]
        public void MarketSellTest_InsufficientHoldings()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 100m);

            var response = Market.MarketSell(_user, _stock, 10);

            response.Should().BeEquivalentTo(
                new SellOrderFailedResponse
                {
                    Reason = SellOrderFailedResponse.FailureReason.InsufficientHoldings
                });
        }

        [TestMethod]
        public void MarketSellTest_NoBuyers()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 100m, new Dictionary<Stock, int>
            {
                {_stock, 10 }
            });

            var response = Market.MarketSell(_user, _stock, 10);

            response.Should().BeEquivalentTo(
                new SellOrderResponse
                {
                    Filled = new FillDetail[0],
                    Unfilled = 10
                });
        }

        [TestMethod]
        public void MarketSellTest_CorrectHoldingsAllocated()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 1000m, new Dictionary<Stock, int>
            {
                {_stock, 10 }
            });
            var buyer = new User { UserId = Guid.NewGuid() };
            RegisterUser(buyer, 1000m,
                buyOrders: new Dictionary<Stock, int>
                {
                    { _stock, 100 }
                });

            Market.MarketSell(_user, _stock, 10);

            Assert.AreEqual(0, Resolve<IHoldingsProvider>().GetHolding(_user, _stock));
            Assert.AreEqual(10, Resolve<IHoldingsProvider>().GetHolding(buyer, _stock));
        }

        [TestMethod]
        public void MarketSellTest_CorrectFundsAllocated()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 0m, new Dictionary<Stock, int>
            {
                {_stock, 10 }
            });
            var buyer = new User { UserId = Guid.NewGuid() };
            RegisterUser(buyer, 100m,
                buyOrders: new Dictionary<Stock, int>
                {
                    { _stock, 10 }
                });

            Market.MarketSell(_user, _stock, 10);

            Assert.AreEqual(100m, Resolve<IBankingProvider>().GetAccount(_user).Balance);
            Assert.AreEqual(0m, Resolve<IBankingProvider>().GetAccount(buyer).Balance);
        }

        [TestMethod]
        public void MarketSellTest_UnfilledOrder()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 100m, new Dictionary<Stock, int>
            {
                {_stock, 200 }
            });
            RegisterUser(new User { UserId = Guid.NewGuid() }, 100m,
                buyOrders: new Dictionary<Stock, int>
                {
                    { _stock, 100 }
                });
            RegisterUser(new User { UserId = Guid.NewGuid() }, 100m,
                buyOrders: new Dictionary<Stock, int>
                {
                    { _stock, 70 }
                });

            Market.MarketSell(_user, _stock, 200);

            Assert.AreEqual(30, Resolve<IHoldingsProvider>().GetHolding(_user, _stock));
            Assert.AreEqual(30, Resolve<IOrderProvider>().GetSellOrders(_stock).Sum(x => x.Quantity));
        }

        [TestMethod]
        public void MarketSellTest_ExcessBuyOrders()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 100m, new Dictionary<Stock, int>
            {
                {_stock, 10 }
            });
            RegisterUser(new User { UserId = Guid.NewGuid() }, 100m,
                buyOrders: new Dictionary<Stock, int>
                {
                    { _stock, 100 }
                });

            Market.MarketSell(_user, _stock, 10);

            Assert.AreEqual(90, Resolve<IOrderProvider>().GetBuyOrders(_stock).Sum(x => x.Quantity));
        }

        [TestMethod]
        public void MarketBuyTest_OrdersTimestamped()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 100m);

            var now = Resolve<IDateTimeSource>().Now;
            Market.MarketBuy(_user, _stock, 1);
            Resolve<IDateTimeSource>().Fastforward(TimeSpan.FromSeconds(5));
            Market.MarketBuy(_user, _stock, 1);

            var orders = Resolve<IOrderProvider>().GetBuyOrders(_stock);
            orders.Select(x => x.Timestamp.ToString("hh:mm:ss")).Should().BeEquivalentTo(
                new[]
                {
                    now.ToString("hh:mm:ss"),
                    now.AddSeconds(5).ToString("hh:mm:ss")
                });
        }
    }
}
