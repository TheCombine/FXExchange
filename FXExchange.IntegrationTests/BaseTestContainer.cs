using Autofac;
using FXExchange;
using FXExchange.Business;
using FXExchange.DB;
using NUnit.Framework;

namespace FxExchange.UnitTests
{
    public abstract class BaseTestContainer
    {
        protected IContainer Container { get; private set; }

        [SetUp]
        public void Setup()
        {
            Container = CreateContainer();
        }

        private static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            ConfigureServices(builder);
            return builder.Build();
        }

        private static void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterModule<UIModule>();
            builder.RegisterModule<BusinessModule>();
            builder.RegisterModule<DBModule>();
        }
    }
}