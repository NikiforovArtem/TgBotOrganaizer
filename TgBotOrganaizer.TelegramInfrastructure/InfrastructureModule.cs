using Autofac;
using TgBotOrganaizer.Core.Interfaces;
using TgBotOrganaizer.Infrastructure.InfrastructureMongo;

namespace TgBotOrganaizer.Infrastructure
{
    public class InfrastructureModule : ConfiguredModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            MongoDbInitializer.Initialize();

            var mongoDbSettings = Configuration.GetSection("MongoDbSettings");

            builder.RegisterType<ArticleMongoRepository>()
                .As<IArticleRepository>()
                .WithParameter("connectionString", mongoDbSettings["ConnectionString"])
                .WithParameter("databaseName", mongoDbSettings["DatabaseName"])
                .InstancePerLifetimeScope();
        }
    }
}
