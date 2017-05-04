using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzureStorage;
using Common.Log;
using Lykke.Service.EmailFormatter.Core.Settings;
using Lykke.Service.EmailFormatter.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EmailFormatter.Controllers
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
        public async Task<EmailFormatResponse> Format(EmailFormatRequest request)
        {
            if (!TryValidateModel(request))
                throw new ArgumentException(nameof(request));

            try
            {
                var template = _partnerTemplateSettings[request.PartnerId, $"{request.CaseId}_{request.Language}"];
                if (null == template)
                    throw new Exception($"Unable to find email template {request.CaseId} ({request.Language}) for partner {request.PartnerId}");

                MatchEvaluator matchEvaluator = delegate (Match match)
                {
                    var key = match.Groups[1].Value;
                    if(null == request.Parameters || !request.Parameters.ContainsKey(key))
                        throw new KeyNotFoundException($"Unable to find parameter {key} required by email template {request.CaseId} ({request.Language}) for partner {request.PartnerId}");
                    return request.Parameters[match.Groups[1].Value].ToString();
                };

                return new EmailFormatResponse
                {
                    TextBody =
                        string.IsNullOrWhiteSpace(template.TextTemplate)
                            ? null
                            : ParameterRegex.Replace(template.TextTemplate, matchEvaluator),
                    HtmlBody =
                        string.IsNullOrWhiteSpace(template.HtmlTemplate)
                            ? null
                            : ParameterRegex.Replace(template.HtmlTemplate, matchEvaluator)
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
