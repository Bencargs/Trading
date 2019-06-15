using Autofac;
using Contracts.Providers;
using Exchange;
using Exchange.Providers;
using Exchange.Services;
using MarketService.Controllers;

namespace MarketServiceTests
{
    public class TestHarness
    {
        protected T Resolve<T>() => _container.Resolve<T>();
        protected AccountController AccountController;

        private readonly IContainer _container;

        protected TestHarness()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<DateTimeSource>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<BankingProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<HoldingsProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<SharesProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<OrderProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<AccountProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            _container = builder.Build();

            var accountProvider = Resolve<IAccountProvider>();
            var bankingProvider = Resolve<IBankingProvider>();
            var holdingsProvider = Resolve<IHoldingsProvider>();
            var account = new Account(accountProvider);
            AccountController = new AccountController(
                account,
                bankingProvider,
                holdingsProvider);
        }
    }
}
