using Contracts.Models;
using Contracts.Providers;
using Contracts.Responses;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ExchangeTests.Services
{
    [TestClass]
    public class LimitTest : TestHarness
    {
        private readonly Stock _stock = new Stock { Ticker = "ABC" };
        private readonly User _user = new User { UserId = Guid.NewGuid(), Username = "TestUser" };

        [TestMethod]
        public void LimitBuyTest_InsufficientFunds()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 0m);
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

            var response = Market.LimitBuy(_user, _stock, 100, 11m);

            response.Should().BeEquivalentTo(new BuyOrderFailedResponse
            {
                Reason = BuyOrderFailedResponse.FailureReason.InsufficientFunds
            });
        }

        [TestMethod]
        public void LimitBuyTest_CorrectResponseDetails()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 1100m);
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

            var response = Market.LimitBuy(_user, _stock, 100, 10m);

            response.Should().BeEquivalentTo(new
            {
                Filled = new[]
                {
                    new
                    {
                        Stock = _stock,
                        Quantity = 10,
                        Price = 10m
                    }
                },
                Unfilled = 90
            });
        }

        [TestMethod]
        public void LimitBuyTest_WhereUnfilledBuy()
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

            Market.LimitBuy(_user, _stock, 100, 10m);

            var expected = Resolve<IOrderProvider>().GetBuyOrders(_stock);
            expected.Should().BeEquivalentTo(
                new
                {
                    Direction = OrderDirection.Buy,
                    Type = OrderType.Limit,
                    Owner = _user,
                    Stock = _stock,
                    Quantity = 95,
                });
        }

        [TestMethod]
        public void LimitBuyTest_CorrectFundsAllocated()
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

            Market.LimitBuy(_user, _stock, 100, 10m);

            Assert.AreEqual(900m, Resolve<IBankingProvider>().GetAccount(_user).Balance);
            Assert.AreEqual(100m, Resolve<IBankingProvider>().GetAccount(seller).Balance);
        }

        [TestMethod]
        public void LimitBuyTest_LimitPrice()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 1000m);
            var seller = new User { UserId = Guid.NewGuid() };
            RegisterUser(seller, 0m,
                holdings: new Dictionary<Stock, int>
                {
                    { _stock, 20 }
                });

            Market.LimitSell(seller, _stock, 10, 10m);
            Market.LimitSell(seller, _stock, 10, 11m);
            var response = Market.LimitBuy(_user, _stock, 100, 10m);

            response.Should().BeEquivalentTo(new
            {
                Filled = new[]
                {
                    new
                    {
                        Stock = _stock,
                        Quantity = 10,
                        Price = 10m
                    }
                },
                Unfilled = 90
            });
        }

        [TestMethod]
        public void LimitSellTest_LimitPrice()
        {
            RegisterShares(_stock, 10m);
            RegisterUser(_user, 0m, holdings: new Dictionary<Stock, int>
                {
                    { _stock, 20 }
                });
            var buyer = new User { UserId = Guid.NewGuid() };
            RegisterUser(buyer, 1000m);

            Market.LimitBuy(buyer, _stock, 10, 9m);
            Market.LimitBuy(buyer, _stock, 10, 10m);
            var response = Market.LimitSell(_user, _stock, 20, 10m);

            response.Should().BeEquivalentTo(new
            {
                Filled = new[]
                {
                    new
                    {
                        Owner = buyer,
                        Stock = _stock,
                        Quantity = 10,
                        Price = 10m
                    }
                },
                Unfilled = 10
            });
        }
    }
}
