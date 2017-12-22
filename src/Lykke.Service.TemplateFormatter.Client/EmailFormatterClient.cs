using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.EmailSender;
using Lykke.Service.TemplateFormatter.AutorestClient;
using Newtonsoft.Json;

namespace Lykke.Service.TemplateFormatter.Client
{
    public class FormatterFormatter : ITemplateFormatter
    {
        private readonly IEmailFormattingService _service;
        private readonly ILog _log;

        public FormatterFormatter(string serviceUrl, ILog log)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            _log = log ?? throw new ArgumentNullException(nameof(log));
            _service = new EmailFormattingService(new Uri(serviceUrl));
        }

        public async Task<EmailMessage> FormatAsync(string caseId, string partnerId, string language, object model = null)
        {
            if (string.IsNullOrWhiteSpace(caseId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(caseId));
            if (string.IsNullOrWhiteSpace(partnerId))
                partnerId = "Lykke";
            if (string.IsNullOrWhiteSpace(language))
                language = "EN";
            IDictionary<string, string> parameters;
            if (null != model)
            {
                parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(model));
            }
            else
            {
                parameters = new Dictionary<string, string>();
            }
            try
            {
                return (await _service.ApiFormatterByCaseIdByPartnerIdByLanguagePostAsync(caseId, partnerId, language, parameters)).EmailMessage;
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(EmailFormatter), nameof(FormatterFormatter), nameof(FormatAsync), ex);
                throw;
            }
        }

        public Task<TemplateModelTypeValidationResult> ValidateTemplateModelTypeAsync(Type templateModelType, string templateName)
        {
            throw new NotImplementedException();
        }

        public Task<TemplateFormattingResult> FormatAsync(string templateModelJson, string templateName, string partnerId, string language)
        {
            throw new NotImplementedException();
        }
    }
}