using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Configuration;

namespace TgBotOrganaizer.Common
{
    public static class ContainerBuilderExtension
    {
        public static ContainerBuilder RegisterConfiguredModulesFromAssemblyContaining<TType>(this ContainerBuilder builder, IConfiguration configuration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var metaBuilder = new ContainerBuilder();

            metaBuilder.RegisterInstance(configuration);
            metaBuilder.RegisterAssemblyTypes(typeof(TType).GetTypeInfo().Assembly)
                .AssignableTo<IModule>()
                .As<IModule>()
                .PropertiesAutowired();

            using var metaContainer = metaBuilder.Build();
            foreach (var module in metaContainer.Resolve<IEnumerable<IModule>>())
            {
                builder.RegisterModule(module);
            }

            return builder;
        }
    }
}
