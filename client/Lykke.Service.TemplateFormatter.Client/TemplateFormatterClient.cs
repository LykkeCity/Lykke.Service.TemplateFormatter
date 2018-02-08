using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.TemplateFormatter.Client.AutorestClient;
using Lykke.Service.TemplateFormatter.Client.AutorestClient.Models;
using Newtonsoft.Json;

namespace Lykke.Service.TemplateFormatter.Client
{
    /// <summary>
    /// TemplateFormatter Client
    /// </summary>
    internal class TemplateFormatterClient : ITemplateFormatter
    {
        private readonly ITemplateFormatterAPI _service;
        private readonly ILog _log;

        internal TemplateFormatterClient(string serviceUrl, ILog log)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            _log = log ?? throw new ArgumentNullException(nameof(log));
            _service = new TemplateFormatterAPI(new Uri(serviceUrl));
        }

        /// <summary>
        /// Formats an email template according to parameters
        /// </summary>
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
                await _log.WriteErrorAsync(nameof(TemplateFormatterClient), nameof(FormatAsync), ex);
                throw;
            }
        }
    }

}
