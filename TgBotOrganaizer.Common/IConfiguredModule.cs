using Autofac;
using Microsoft.Extensions.Configuration;

namespace TgBotOrganaizer
{
    public interface IConfiguredModule
    {
        IConfiguration Configuration { get; set; }
    }

    public abstract class ConfiguredModule : Module, IConfiguredModule
    {
        public IConfiguration Configuration { get; set; }
    }
}
