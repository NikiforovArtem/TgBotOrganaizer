namespace TgBotOrganaizer.InfrastructureMongo
{
    public interface IDotNetTheoryMongoStoreSettings
    {
        string ConnectionString { get; set; }

        string DatabaseName { get; set; }
    }

    internal class DotNetTheoryMongoStoreSettings : IDotNetTheoryMongoStoreSettings
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }
    }
}
