using Autofac;
using FXExchange.DB.Currencies;
using FXExchange.DB.ExchangeRates;

namespace FXExchange.DB
{
    public class DBModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<CurrencyRepository>().As<ICurrencyRepository>();
            builder.RegisterType<ExchangeRateRepository>().As<IExchangeRateRepository>();
        }
    }
}
