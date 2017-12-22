using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Lykke.Service.TemplateFormatter.Client
{
    /// <summary>
    /// Template formatting result message
    /// </summary>
    public class TemplateFormattingResult
    {
        /// <summary>
        /// Template name used for formatting
        /// </summary>
        [NotNull]
        [Required]
        public string TemplateName { get; set; }

        /// <summary>
        /// Partner whos template was finally used for formatting. Null if default template was used
        /// </summary>
        [NotNull]
        [Required]
        public string PartnerId { get; set; }

        /// <summary>
        /// Template language used for formatting
        /// </summary>
        [NotNull]
        [Required]
        public string Language { get; set; }

        /// <summary>
        /// Subject
        /// </summary>
        [CanBeNull]
        public string Subject { get; set; }

        /// <summary>
        /// Text body
        /// </summary>
        [CanBeNull]
        public string Text { get; set; }

        /// <summary>
        /// HTML body
        /// </summary>
        [CanBeNull]
        public string Html { get; set; }

        public TemplateFormattingResult([NotNull] string templateName, [NotNull] string language)
        {
            if (string.IsNullOrWhiteSpace(templateName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(templateName));
            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(language));

            TemplateName = templateName;
            Language = language;
        }
    }
}