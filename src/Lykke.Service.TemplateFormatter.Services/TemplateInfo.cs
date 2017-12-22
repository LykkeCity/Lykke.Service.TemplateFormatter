using System;
using JetBrains.Annotations;

namespace Lykke.Service.TemplateFormatter.Services
{
    public class TemplateInfo
    {
        private readonly string _partnerId;
        private readonly string _language;
        private readonly string _templateName;

        public TemplateInfo([NotNull] string partnerId, [NotNull] string language, [NotNull] string templateName)
        {
            if (String.IsNullOrWhiteSpace(partnerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(partnerId));
            if (String.IsNullOrWhiteSpace(language))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(language));
            if (String.IsNullOrWhiteSpace(templateName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(templateName));

            _partnerId = partnerId;
            _language = language;
            _templateName = templateName;
        }

        public void AddPartType(string partType)
        {
        }

        public bool HasSubject { get; set; }

        public bool HasHtml { get; set; }

        public bool HasText { get; set; }
    }
}