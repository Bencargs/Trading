using Autofac;
using Contracts.Providers;
using Exchange.Providers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ExchangeTests.Providers
{
    [TestClass]
    public class AccountProviderTest
    {
        private IAccountProvider _provider;

        public AccountProviderTest()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<BankingProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<HoldingsProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<OrderProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<SharesProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            var container = builder.Build();

            _provider = new AccountProvider(
                container.Resolve<IBankingProvider>(),
                container.Resolve<IHoldingsProvider>());
        }

        [TestMethod]
        public void CreateTest()
        {
            var userId = Guid.NewGuid();
            var response = _provider.RegisterUser(userId, "TestUser");

            var user = _provider.GetUser(userId);

            Assert.AreEqual(response, userId);
            user.Should().BeEquivalentTo(
                new
                {
                    UserId = userId,
                    Username = "TestUser"
                });
        }
    }
}
