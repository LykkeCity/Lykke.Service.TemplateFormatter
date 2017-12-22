using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace Lykke.Service.TemplateFormatter.Services
{
    public class Template
    {
        public Template([NotNull] string partnerId, [NotNull] string language, [NotNull] string templateName)
        {
            if (String.IsNullOrWhiteSpace(partnerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(partnerId));
            if (String.IsNullOrWhiteSpace(language))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(language));
            if (String.IsNullOrWhiteSpace(templateName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(templateName));

            PartnerId = partnerId;
            Language = language;
            Name = templateName;
        }

        [NotNull] public string PartnerId { get; }
        [NotNull] public string Language { get; }
        [NotNull] public string Name { get; }

        [DisplayName("subject")]
        public string Subject { get; set; }

        [DisplayName("HTML body")]
        public string HtmlBody { get; set; }

        [DisplayName("text body")]
        public string TextBody { get; set; }
    }
}