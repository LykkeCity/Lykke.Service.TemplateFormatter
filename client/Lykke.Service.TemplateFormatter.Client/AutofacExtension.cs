using System;
using Autofac;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.TemplateFormatter.Client;

namespace Lykke.Service.TemplateFormatter
{
    /// <summary>
    /// TemplateFormatter Autofac extension
    /// </summary>
    public static class AutofacExtension
    {
        /// <summary>
        /// Adds Template Formatter client to the ContainerBuilder.
        /// </summary>
        /// <param name="builder">ContainerBuilder instance.</param>
        /// <param name="serviceUrl">Effective Template Formatter service location.</param>
        /// <param name="log">Logger.</param>
        [Obsolete("Please, use the overload without explicitly passed logger.")]
        public static void RegisterTemplateFormatter(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterInstance(new TemplateFormatterClient(serviceUrl, log)).As<ITemplateFormatter>().SingleInstance();
        }

        /// <summary>
        /// Adds Template Formatter client to the ContainerBuilder.
        /// </summary>
        /// <param name="builder">ContainerBuilder instance. The implementation of ILogFactory should be already injected.</param>
        /// <param name="serviceUrl">Effective Template Formatter service location.</param>
        public static void RegisterTemplateFormatter(this ContainerBuilder builder, string serviceUrl)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.Register(ctx => new TemplateFormatterClient(
                serviceUrl, 
                ctx.Resolve<ILogFactory>()))
                .As<ITemplateFormatter>()
                .SingleInstance();
        }
    }
}

