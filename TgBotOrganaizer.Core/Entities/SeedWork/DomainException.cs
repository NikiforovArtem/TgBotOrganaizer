using System;

namespace TgBotOrganaizer.Core.Entities.SeedWork
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }
    }
}
