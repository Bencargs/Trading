using Contracts.Models;
using Contracts.Providers;
using Market.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarketTest.Providers
{
    [TestClass]
    public class SharesProviderTest
    {
        private readonly Stock _stock = new Stock { Ticker = "ABC" };
        private readonly ISharesProvider _provider = new SharesProvider();

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
            _provider.UpdateLastPrice(_stock, 20m);

            var actual = _provider.GetLastPrice(_stock);

            Assert.AreEqual(20m, actual);
        }
    }
}
