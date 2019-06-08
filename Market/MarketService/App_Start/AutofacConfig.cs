using Autofac;
using Autofac.Integration.WebApi;
using Contracts;
using Contracts.Providers;
using Exchange;
using Exchange.Providers;
using Exchange.Services;
using MarketService.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace MarketService
{
    public class AutofacConfig
    {
        public static IContainer Container;

        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);

            builder.RegisterType<DateTimeSource>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<AccountProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<BankingProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<HoldingsProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<OrderProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<SharesProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<Market>().InstancePerRequest();
            builder.RegisterType<Account>().InstancePerRequest();

            Container = builder.Build();

            var resolver = new AutofacWebApiDependencyResolver(Container);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }
    }
}