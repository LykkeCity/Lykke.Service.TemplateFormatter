using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using AzureStorage.Blob;
using Lykke.Service.TemplateFormatter.Services;

namespace Lykke.Service.TemplateFormatter.Tests
{
    public class TestBasicFormattingLogic
    {
        private readonly TemplateManager _templateManager;

        private readonly Task _initialize;
        private BasicFormattingLogic _basicFormattingLogic;

        public TestBasicFormattingLogic()
        {
            var blob = new AzureBlobInMemory();
            _initialize = blob.LoadBlobFilesAsync(TemplateManager.TemplatesContainerName);
            _templateManager = new TemplateManager(blob);
            _basicFormattingLogic = new BasicFormattingLogic();
        }

        [Theory]
        [InlineData("AlpineVault", "EN", "Test3", new[] { "ClientName", "BankAddress", "Year", "bic" }, new[] { "Bic", "AccountNumber", "AccountName", "CompanyAddress", "PurposeOfPayment", "AssetSymbol", "Amount", "CorrespondentAccount", "AssetId" })]
        public async Task TestValidate(string partnerId, string language, string templateName, string[] parameterNames, string[] expectedMissingParameterNames)
        {
            await _initialize;
            var template = await _templateManager.GetTemplateAsync(partnerId, language, templateName);
            Assert.NotNull(template);
            var missingParameterNames = _basicFormattingLogic.Validate(template, parameterNames).Select(x => x.ParameterName).Distinct().ToArray();
            Assert.Equal(expectedMissingParameterNames.OrderBy(x => x), missingParameterNames.OrderBy(x => x));
        }


        [Theory]
        [InlineData("AlpineVault", "EN", "Test3",
            new[] { "John Smith" },
            new[] { "4356456", "ˆ", "EUR" },
            new[] { "" })]
        public async Task TestFormat(string partnerId, string language, string templateName, string[] subjectSubstrings, string[] htmlSubstrings, string[] textSubstrings)
        {
            var parameters = new Dictionary<string, string>
            {
                {"ClientName", "John Smith"},
                {"BankAddress", "dgsdfg"},
                {"Year", "2017"},
                {"Bic", "123456"},
                {"AccountNumber", "234"},
                {"AccountName", "4356456"},
                {"CompanyAddress", "9823982673"},
                {"PurposeOfPayment", "regregrr"},
                {"AssetSymbol", "ˆ"},
                {"Amount", "232355"},
                {"CorrespondentAccount", "asd"},
                {"AssetId", "EUR"}
            };
            await _initialize;

            var template = await _templateManager.GetTemplateAsync(partnerId, language, templateName);
            Assert.NotNull(template);
            var formattingResult = _basicFormattingLogic.Format(template, parameters);
            foreach (var subjectSubstring in subjectSubstrings.Where(x => !string.IsNullOrWhiteSpace(x)))
                Assert.Contains(subjectSubstring, formattingResult.Subject);
            foreach (var htmlSubstring in htmlSubstrings.Where(x => !string.IsNullOrWhiteSpace(x)))
                Assert.Contains(htmlSubstring, formattingResult.HtmlBody);
            foreach (var textSubstring in textSubstrings.Where(x => !string.IsNullOrWhiteSpace(x)))
                Assert.Contains(textSubstring, formattingResult.TextBody);
        }
    }
}
