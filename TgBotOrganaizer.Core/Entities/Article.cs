using System;
using System.Collections.Generic;
using System.Linq;
using TgBotOrganaizer.Core.Entities.SeedWork;

namespace TgBotOrganaizer.Core.Entities
{
    public class Article : Entity, IAggregateRoot
    {
        private List<PhotoItem> photoItems = new();

        public Article(string theme, string text, string id)
        {
            this.Id = id;
            this.Theme = !string.IsNullOrEmpty(theme) ? theme : throw new DomainException("У статьи должна быть задана тема");
            this.Text = text;
        }

        public string Id { get; }

        public string Theme { get; }

        public IReadOnlyCollection<PhotoItem> PhotoItems
        {
            get => photoItems;
            //Need to MongoDbDriver can deserealize
            private set => this.photoItems = value.ToList();
        }

        public string Text { get; private set; }

        public void AddPhotoItem(string caption, string externalId)
        {
            this.photoItems.Add(new PhotoItem(caption, externalId));
        }

        public void AddText(string newText)
        { 
            this.Text += Environment.NewLine + newText;
        }
    }
}