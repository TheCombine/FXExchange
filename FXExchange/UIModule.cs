using Autofac;
using FXExchange.CommandLineArguments;
using FXExchange.Services;

namespace FXExchange
{
    public class UIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ArgumentsParser>().As<IArgumentsParser>();
            builder.RegisterType<AppRunner>().As<IAppRunner>();
            builder.RegisterType<ResultWriter>().As<IResultWriter>();
        }
    }
}
