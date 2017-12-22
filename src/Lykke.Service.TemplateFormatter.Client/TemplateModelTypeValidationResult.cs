using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.TemplateFormatter.Client
{
    /// <summary>
    /// Message model type validation result
    /// </summary>
    public class TemplateModelTypeValidationResult
    {
        /// <summary>
        /// True if current message model type schema fits template requirements
        /// </summary>
        public bool IsValid { get; private set; }

        public TemplateModelTypeValidationError[] Errors { get; private set; }
    }

    public class TemplateModelTypeValidationError
    {
        public string PartnerId { get; private set; }
    }
}