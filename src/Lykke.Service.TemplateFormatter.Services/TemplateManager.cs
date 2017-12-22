using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzureStorage;
using JetBrains.Annotations;

namespace Lykke.Service.TemplateFormatter.Services
{
    public class TemplateManager
    {
        public const string TemplatesContainerName = "templates";

        private const string PartnerIdGroupName = "partnerId";
        private const string LanguageGroupName = "language";
        private const string TemplateNameGroupName = "templateName";
        private const string TemplatePartTypeGroupName = "partType";

        private static readonly Regex TemplatePathRegex = new Regex($"^(?<{PartnerIdGroupName}>[^/]+)/(?<{LanguageGroupName}>[^/]+)/(?<{TemplateNameGroupName}>[^/]+).(?<{TemplatePartTypeGroupName}>{TemplateInfo.TemplatePartTypeSubject}|{TemplateInfo.TemplatePartTypeHtml}|{TemplateInfo.TemplatePartTypeText})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IBlobStorage _blob;
        
        public TemplateManager([NotNull] IBlobStorage blob)
        {
            _blob = blob ?? throw new ArgumentNullException(nameof(blob));
        }

        public async Task<IEnumerable<TemplateInfo>> GetTemplatesAsync([NotNull] string templateName)
        {
            if (string.IsNullOrWhiteSpace(templateName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(templateName));

            var result = new Dictionary<string, TemplateInfo>();

            foreach (var templateFilePath in await _blob.GetListOfBlobKeysAsync(TemplatesContainerName))
            {
                var match = TemplatePathRegex.Match(templateFilePath);
                if (!match.Success || match.Groups[TemplateNameGroupName].Value != templateName)
                    continue;

                var partnerId = match.Groups[PartnerIdGroupName].Value;
                var language = match.Groups[LanguageGroupName].Value;
                var templateKey = TemplateInfo.GetKey(partnerId, language, templateName);
                TemplateInfo templateInfo;
                if (result.ContainsKey(templateKey))
                {
                    templateInfo = result[templateKey];
                }
                else
                {
                    templateInfo = new TemplateInfo(partnerId, language, templateName);
                    result.Add(templateKey, templateInfo);
                }

                templateInfo.AddPartType(match.Groups[TemplatePartTypeGroupName].Value.ToLower());
            }

            return result.Values;
        }
    }

    public class TemplateInfo
    {
        public const string TemplatePartTypeHtml = "html";
        public const string TemplatePartTypeText = "txt";
        public const string TemplatePartTypeSubject = "json";

        private readonly string _partnerId;
        private readonly string _language;
        private readonly string _templateName;

        public TemplateInfo([NotNull] string partnerId, [NotNull] string language, [NotNull] string templateName)
        {
            if (String.IsNullOrWhiteSpace(partnerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(partnerId));
            if (String.IsNullOrWhiteSpace(language))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(language));
            if (String.IsNullOrWhiteSpace(templateName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(templateName));

            _partnerId = partnerId;
            _language = language;
            _templateName = templateName;
        }

        public string GetKey()
        {
            return GetKey(_partnerId, _language, _templateName);
        }

        public static string GetKey(string partnerId, string language, string templateName)
        {
            return $"{partnerId}/{language}/{templateName}";
        }

        public void AddPartType(string partType)
        {
            switch (partType)
            {
                case TemplatePartTypeSubject:
                    HasSubject = true;
                    break;
                case TemplatePartTypeHtml:
                    HasHtml = true;
                    break;
                case TemplatePartTypeText:
                    HasText = true;
                    break;
                default:
                    throw new Exception($"Unknown part type {partType}");
            }
        }

        public bool HasSubject { get; set; }

        public bool HasHtml { get; set; }

        public bool HasText { get; set; }
    }
}
