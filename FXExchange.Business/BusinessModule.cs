using Autofac;
using FXExchange.Business.MoneyConverter;

namespace FXExchange.Business
{
    public class BusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<MoneyConverterService>().As<IMoneyConverterService>();
        }
    }
}
