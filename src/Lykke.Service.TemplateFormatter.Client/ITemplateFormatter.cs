using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.EmailSender;

namespace Lykke.Service.TemplateFormatter.Client
{
    public interface ITemplateFormatter
    {
        /// <summary>
        /// Validate if template model type fits template requirements
        /// </summary>
        /// <param name="templateModelType">Template model type to validate</param>
        /// <param name="templateName">Template name</param>
        /// <returns>Returns template model type validation result. Use ITemplateModelTypeValidationResult.IsValid to make sure models of type passed can be used for formatting</returns>
        /// <exception cref="TemplateNotFoundException">Template <paramref name="templateName"/> not found</exception>
        [NotNull] Task<TemplateModelTypeValidationResult> ValidateTemplateModelTypeAsync([NotNull] Type templateModelType, [NotNull] string templateName);

        /// <summary>
        /// Format message using model and template
        /// </summary>
        /// <param name="templateModelJson">Template model to format</param>
        /// <param name="templateName">Template name to use</param>
        /// <param name="partnerId">Partner whos template to use. If template is not found for partner, default template is used</param>
        /// <param name="language">Template language to use. If template is not found for partner, EN template is used</param>
        /// <returns></returns>
        /// <exception cref="TemplateNotFoundException">Template <paramref name="templateName"/> not found for specified <paramref name="partnerId"/> and <paramref name="language"/> or any fallback</exception>
        [NotNull] Task<TemplateFormattingResult> FormatAsync([NotNull] string templateModelJson, [NotNull] string templateName, [NotNull] string partnerId, [NotNull] string language);
    }
}