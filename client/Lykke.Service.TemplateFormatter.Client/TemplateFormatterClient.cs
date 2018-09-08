using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.EmailSender;
using Lykke.Service.TemplateFormatter.Client.AutorestClient;
using Newtonsoft.Json;

namespace Lykke.Service.TemplateFormatter.Client
{
    /// <summary>
    /// TemplateFormatter Client
    /// </summary>
    internal class TemplateFormatterClient : ITemplateFormatter, IDisposable
    {
        private ITemplateFormatterAPI _service;
        private readonly ILog _log;

        [Obsolete("Please, use the overload which consumes ILogFactory instead.")]
        internal TemplateFormatterClient(string serviceUrl, ILog log)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            _log = log ?? throw new ArgumentNullException(nameof(log));
            _service = new TemplateFormatterAPI(new Uri(serviceUrl));
        }

        internal TemplateFormatterClient(string serviceUrl, ILogFactory logFactory)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            if (logFactory == null)
                throw new ArgumentNullException(nameof(logFactory));
            _log = logFactory.CreateLog(this);

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

        private bool isDisposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    _service?.Dispose();
                    _service = null;
                }
                isDisposed = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
