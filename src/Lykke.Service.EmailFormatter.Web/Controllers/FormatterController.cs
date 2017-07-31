using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzureStorage;
using Common.Log;
using Lykke.Service.EmailFormatter.Web.Models;
using Lykke.Service.EmailFormatter.Web.Settings;
using Lykke.Service.EmailSender;
using Lykke.WebExtensions;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EmailFormatter.Web.Controllers
{
    public class FormatterController : Controller
    {
        private static readonly Regex ParameterRegex = new Regex(@"@\[([^\]]+)\]");

        private readonly INoSQLTableStorage<PartnerTemplateSettings> _partnerTemplateSettings;
        private readonly ILog _log;

        public FormatterController(INoSQLTableStorage<PartnerTemplateSettings> partnerTemplateSettings, ILog log)
        {
            _partnerTemplateSettings = partnerTemplateSettings;
            _log = log;
        }

        [HttpPost]
        [Route("api/[controller]/{caseId}/{partnerId}/{language}")]
        [ValidOnlyFilter]
        public async Task<EmailFormatResponse> Format(string caseId, string partnerId, string language, [FromBody]Dictionary<string, string> parameters)
        {
            if(null == parameters)
                parameters = new Dictionary<string, string>();

            try
            {
                var template = _partnerTemplateSettings[partnerId, $"{caseId}_{language}"];
                if (null == template)
                    throw new Exception($"Unable to find email template {caseId} ({language}) for partner {partnerId}");

                string MatchEvaluator(Match match)
                {
                    var key = match.Groups[1].Value;
                    if (null != parameters && parameters.ContainsKey(key))
                        return parameters[key];
                    throw new KeyNotFoundException($"Unable to find parameter {key} required by email template {caseId} ({language}) for partner {partnerId}");
                }

                return new EmailFormatResponse
                {
                    EmailMessage = new EmailMessage
                    {
                        Subject = string.IsNullOrWhiteSpace(template.SubjectTemplate)
                            ? "testSubject"
                            : ParameterRegex.Replace(template.SubjectTemplate, MatchEvaluator),
                        TextBody = string.IsNullOrWhiteSpace(template.TextTemplate)
                            ? null
                            : ParameterRegex.Replace(template.TextTemplate, MatchEvaluator),
                        HtmlBody = string.IsNullOrWhiteSpace(template.HtmlTemplate)
                            ? null
                            : ParameterRegex.Replace(template.HtmlTemplate, MatchEvaluator)
                    }
                };
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(EmailFormatter), nameof(Startup), nameof(Format), ex, DateTime.UtcNow);
                throw;
            }
        }
    }
}
