using TgBotOrganaizer.Core.Entities.SeedWork;

namespace TgBotOrganaizer.Core.Entities
{
    public class PhotoItem : ValueObject
    {
        public PhotoItem(string caption, string externalId)
        {

            this.Caption = caption;
            this.ExternalId = externalId ?? throw new DomainException("Изображение не имеет ссылки на внешнее файловое хранилище"); ;
        }

        public string Caption { get; }


        public string ExternalId { get; }
    }
}
