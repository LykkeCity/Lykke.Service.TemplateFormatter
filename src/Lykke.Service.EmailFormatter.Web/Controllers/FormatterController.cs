using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzureStorage;
using Common.Log;
using Lykke.Service.EmailFormatter.Settings;
using Lykke.Service.EmailFormatter.Web.Models;
using Lykke.WebExtensions;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EmailFormatter.Web.Controllers
{
    [Route("api/[controller]/[action]")]
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
        [ValidOnlyFilter]
        public async Task<EmailFormatResponse> Format(EmailFormatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PartnerId))
                request.PartnerId = "Lykke";
            if (string.IsNullOrWhiteSpace(request.Language))
                request.Language = "EN";
            try
            {
                var template = _partnerTemplateSettings[request.PartnerId, $"{request.CaseId}_{request.Language}"];
                if (null == template)
                    throw new Exception($"Unable to find email template {request.CaseId} ({request.Language}) for partner {request.PartnerId}");

                string MatchEvaluator(Match match)
                {
                    var key = match.Groups[1].Value;
                    if (null != request.Parameters && request.Parameters.ContainsKey(key))
                        return request.Parameters[key];
                    if ("Year" == key)
                        return DateTime.UtcNow.Year.ToString();
                    throw new KeyNotFoundException($"Unable to find parameter {key} required by email template {request.CaseId} ({request.Language}) for partner {request.PartnerId}");
                }

                return new EmailFormatResponse
                {
                    Subject =
                        string.IsNullOrWhiteSpace(template.SubjectTemplate)
                            ? "testEmail"
                            : ParameterRegex.Replace(template.SubjectTemplate, MatchEvaluator),
                    TextBody =
                        string.IsNullOrWhiteSpace(template.TextTemplate)
                            ? null
                            : ParameterRegex.Replace(template.TextTemplate, MatchEvaluator),
                    HtmlBody =
                        string.IsNullOrWhiteSpace(template.HtmlTemplate)
                            ? null
                            : ParameterRegex.Replace(template.HtmlTemplate, MatchEvaluator)
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
