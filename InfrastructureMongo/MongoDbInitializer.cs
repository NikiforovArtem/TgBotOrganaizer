using MongoDB.Bson.Serialization;
using TgBotOrganaizer.Core.Entities;

namespace TgBotOrganaizer.InfrastructureMongo
{
    public static class MongoDbInitializer
    {
        public static void Initialize()
        {
            BsonClassMap.RegisterClassMap<Article>(x =>
            {
                x.AutoMap();
                x.MapMember(a => a.PhotoItems);
                x.SetIdMember(x.GetMemberMap(y => y.Id));
                x.GetMemberMap(y => y.Theme).SetIsRequired(true);
            });
        }
    }
}