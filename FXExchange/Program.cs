using Autofac;
using FXExchange.Business;
using FXExchange.DB;
using FXExchange.Services;
using System.Threading.Tasks;

namespace FXExchange
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            var container = CreateContainer();
            return container.Resolve<IAppRunner>().RunAsync(args);
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