using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lykke.Service.TemplateFormatter.Services
{
    public class BasicFormattingLogic
    {
        private const string ParameterNameGroupName = "ParameterName";
        private static readonly Regex ParameterRegex = new Regex($@"@\[(?<{ParameterNameGroupName}>[^\]]+)\]");

        public FormatResult Format([NotNull] Template template, [NotNull] IDictionary<string, string> parameters)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            string MatchEvaluator(Match match)
            {
                var key = match.Groups[ParameterNameGroupName].Value;
                if (parameters.ContainsKey(key))
                    return parameters[key];
                throw new KeyNotFoundException($"Unable to find parameter {key} required by email template {template.Name} ({template.Language}) for partner {template.PartnerId}");
            }

            return new FormatResult
            {
                Subject = string.IsNullOrWhiteSpace(template.Subject)
                        ? null
                        : ParameterRegex.Replace(template.Subject, MatchEvaluator),
                HtmlBody = string.IsNullOrWhiteSpace(template.HtmlBody)
                    ? null
                    : ParameterRegex.Replace(template.HtmlBody, MatchEvaluator),
                TextBody = string.IsNullOrWhiteSpace(template.TextBody)
                        ? null
                        : ParameterRegex.Replace(template.TextBody, MatchEvaluator)
            };
        }

        public IEnumerable<ParameterValidationError> Validate([NotNull] Template template, [NotNull] ICollection<string> parameterNames)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            if (parameterNames == null) throw new ArgumentNullException(nameof(parameterNames));

            return Validate(template, x => x.Subject, parameterNames)
                .Union(Validate(template, x => x.HtmlBody, parameterNames))
                .Union(Validate(template, x => x.TextBody, parameterNames));
        }

        private IEnumerable<ParameterValidationError> Validate([NotNull] Template template, [NotNull] Expression<Func<Template, string>> parameterExpression, [NotNull] ICollection<string> parameterNames)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            if (parameterExpression == null) throw new ArgumentNullException(nameof(parameterExpression));
            if (parameterNames == null) throw new ArgumentNullException(nameof(parameterNames));

            if (!(parameterExpression.Body is MemberExpression))
                throw new ArgumentException($"{nameof(parameterExpression.Body)} should be of type {nameof(MemberExpression)}", nameof(parameterExpression));

            var templatePart = parameterExpression.Compile().Invoke(template);
            if (string.IsNullOrWhiteSpace(templatePart))
                return Array.Empty<ParameterValidationError>();

            var templatePartDisplayName = GetDisplayName(parameterExpression.Body);
            var requiredParameters = new Collection<string>();
            var match = ParameterRegex.Match(templatePart);
            while (match.Success)
            {
                var requiredParameter = match.Groups[ParameterNameGroupName].Value;
                if(!requiredParameters.Contains(requiredParameter))
                    requiredParameters.Add(requiredParameter);
                match = match.NextMatch();
            }

            return requiredParameters.Where(x => !parameterNames.Contains(x)).Select(x => new ParameterValidationError
            {
                PartnerId = template.PartnerId,
                Language = template.Language,
                TemplateName = template.Name,
                TemplatePart = templatePart,
                ParameterName= x,
                Message = $"Missing required parameter {x} by template {template.Name} ({template.Language}) {templatePartDisplayName} for {template.PartnerId}"
            });
        }

        private string GetDisplayName(Expression parameterExpressionBody)
        {
            var memberExpression = (MemberExpression) parameterExpressionBody;

            var displayNameAttribute = memberExpression.Member.GetCustomAttribute<DisplayNameAttribute>();
            if (null != displayNameAttribute)
                return displayNameAttribute.DisplayName;

            return memberExpression.Member.Name;
        }
    }

    public class ParameterValidationError
    {
        public string PartnerId { get; set; }
        public string Language { get; set; }
        public string TemplateName { get; set; }
        public string TemplatePart { get; set; }
        public string ParameterName { get; set; }
        public string Message { get; set; }
    }
}