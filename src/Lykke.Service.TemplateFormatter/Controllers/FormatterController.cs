using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzureStorage;
using Common.Log;
using Lykke.Service.EmailSender;
using Lykke.Service.TemplateFormatter.Models;
using Lykke.Service.TemplateFormatter.Services;
using Lykke.Service.TemplateFormatter.Web;
using Lykke.Service.TemplateFormatter.Web.Settings;
using Lykke.WebExtensions;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.TemplateFormatter.Controllers
{
    public class FormatterController : Controller
    {
        private const string FallbackLanguage = "EN";
        private const string FallbackPartnerId = "Lykke";

        private readonly TemplateManager _templates;
        private readonly ILog _log;

        public FormatterController(TemplateManager templates, ILog log)
        {
            _templates = templates;
            _log = log;
        }

        [HttpPost]
        [Route("api/[controller]/{templateName}/{partnerId}/{language}")]
        [ValidOnlyFilter]
        public async Task<EmailFormatResponse> Format([FromRoute] string templateName, [FromRoute] string partnerId, [FromRoute] string language, [FromBody] Dictionary<string, string> parameters)
        {
            if(null == parameters)
                parameters = new Dictionary<string, string>();

            try
            {
                var template = await _templates.GetTemplateAsync(partnerId, language, templateName);
                if(null == template && FallbackLanguage != language)
                    template = await _templates.GetTemplateAsync(partnerId, FallbackLanguage, templateName);
                if (null == template && FallbackPartnerId != partnerId)
                    template = await _templates.GetTemplateAsync(FallbackPartnerId, language, templateName);
                if (null == template && FallbackPartnerId != partnerId && FallbackLanguage != language)
                    template = await _templates.GetTemplateAsync(FallbackPartnerId, FallbackLanguage, templateName);
                if (null == template)
                    throw new Exception($"Unable to find email template {templateName} ({language}) for partner {partnerId}");

                var result = new BasicFormattingLogic().Format(template, parameters);
                return new EmailFormatResponse
                {
                    EmailMessage = new EmailMessage
                    {
                        Subject = result.Subject,
                        HtmlBody = result.HtmlBody,
                        TextBody = result.TextBody
                    }
                };
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(TemplateFormatter), nameof(Startup), nameof(Format), ex, DateTime.UtcNow);
                throw;
            }
        }
    }
}
