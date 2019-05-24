﻿using Autofac;
using Contracts.Models;
using Contracts.Providers;
using Market.Providers;
using System;
using System.Collections.Generic;

namespace MarketTest
{
    public class TestHarness
    {
        protected Market.Services.Market Market;
        protected T Resolve<T>() => _container.Resolve<T>();

        private readonly IContainer _container;

        protected TestHarness()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<BankingProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<HoldingsProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<OrderProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<SharesProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            _container = builder.Build();

            SetupTest();
        }

        private void SetupTest()
        {
            var ordersProvider = Resolve<IOrderProvider>();
            var holdingsProvider = Resolve<IHoldingsProvider>();
            var bankingProvider = Resolve<IBankingProvider>();
            var sharesProvider = Resolve<ISharesProvider>();

            Market = new Market.Services.Market(
                ordersProvider,
                holdingsProvider,
                bankingProvider,
                sharesProvider);
        }

        public class OrderProxy
        {
            public int Quantity { get; set; }
            public decimal? Price { get; set; }
        }

        protected void RegisterUser(
                User user,
                decimal? funds = null,
                Dictionary<Stock, int> holdings = null,
                Dictionary<Stock, OrderProxy> sellOrders = null)
        {
            var bankingProvider = Resolve<IBankingProvider>();
            bankingProvider.CreateAccount(user);
            bankingProvider.AddFunds(user, funds ?? 0m);

            var holdingsProvider = Resolve<IHoldingsProvider>();
            holdingsProvider.CreateUser(user);
            if (holdings != null)
            {
                foreach (var h in holdings)
                    holdingsProvider.AddHolding(user, h.Key, h.Value);
            }

            var orderProvider = Resolve<IOrderProvider>();
            if (sellOrders != null)
            {
                foreach (var s in sellOrders)
                    orderProvider.AddSellOrder(user, s.Key, s.Value.Quantity, s.Value.Price);
            }
        }

        protected void RegisterShares(Stock stock, decimal price)
        {
            var sharesProvider = Resolve<ISharesProvider>();
            sharesProvider.AddStock(stock, price);
        }
    }
}
