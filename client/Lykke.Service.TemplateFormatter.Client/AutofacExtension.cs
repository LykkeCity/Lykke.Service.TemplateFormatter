using System;
using Autofac;
using Common.Log;
using Lykke.Service.TemplateFormatter.Client;

namespace Lykke.Service.TemplateFormatter
{
    /// <summary>
    /// TemplateFormatter Autofac extension
    /// </summary>
    public static class AutofacExtension
    {
        /// <summary>
        /// Registers TemplateFormatter service
        /// </summary>
        public static void RegisterTemplateFormatter(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterInstance(new TemplateFormatterClient(serviceUrl, log)).As<ITemplateFormatter>().SingleInstance();
        }
    }
}

