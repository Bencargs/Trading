using Contracts.Models;
using Contracts.Providers;
using Exchange;
using Exchange.Providers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExchangeTests.Providers
{
    [TestClass]
    public class SharesProviderTest
    {
        private readonly Stock _stock = new Stock { Ticker = "ABC" };
        private readonly ISharesProvider _provider = new SharesProvider(new DateTimeSource());

        [TestMethod]
        public void GetStockTest()
        {
            _provider.AddStock(_stock, 5m);

            var actual = _provider.GetLastPrice(_stock);

            Assert.AreEqual(5m, actual);
        }

        [TestMethod]
        public void UpdatedLastPriceTest()
        {
            _provider.AddStock(_stock, 10m);
            var fills = new[]
            {
                new FillDetail
                {
                    Stock = _stock,
                    Quantity = 100,
                    Price = 20m
                }
            };
            _provider.UpdateStock(fills);

            var actual = _provider.GetLastPrice(_stock);

            Assert.AreEqual(20m, actual);
        }
        
        [TestMethod]
        public void GetTrades()
        {
            _provider.AddStock(_stock, 10m);
            var fills = new[]
            {
                new FillDetail
                {
                    Stock = _stock,
                    Quantity = 100,
                    Price = 20m
                }
            };
            _provider.UpdateStock(fills);

            var actual = _provider.GetTrades(_stock);

            actual.Should().BeEquivalentTo(new[]
            {
                new
                {
                    Price = 20m,
                    Volume = 100
                }
            });
        }
    }
}
