using Autofac;
using TgBotOrganaizer.Application;

namespace TgBotOrganaizer
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TelegramMessageHandler>().As<ITelegramMessageHandler>().InstancePerLifetimeScope();
        }
    }
}
