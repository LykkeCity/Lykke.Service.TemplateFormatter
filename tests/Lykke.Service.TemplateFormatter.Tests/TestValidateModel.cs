using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using AzureStorage.Blob;
using Lykke.Service.TemplateFormatter.Services;

namespace Lykke.Service.TemplateFormatter.Tests
{
    public class TestTemplateManager
    {
        private readonly TemplateManager _templateManager;

        private readonly Task _initialize;

        public TestTemplateManager()
        {
            var blob = new AzureBlobInMemory();
             _initialize = blob.LoadBlobFilesAsync(TemplateManager.TemplatesContainerName);
            _templateManager = new TemplateManager(blob);
        }

        [Theory]
        [InlineData("Test1", 1)]
        [InlineData("Test2", 2)]
        [InlineData("Test3", 3)]
        public async Task TestGetTemplates(string templateName, int expectedCount)
        {
            await _initialize;
            Assert.Equal(expectedCount, (await _templateManager.GetTemplatesAsync(templateName)).Count());
        }

        [Theory]
        [InlineData("Test1", false)]
        [InlineData("Test2", true)]
        [InlineData("Test3", true)]
        public async Task TestHasSubject(string templateName, bool expectedValue)
        {
            await _initialize;

            foreach (var templateInfo in await _templateManager.GetTemplatesAsync(templateName))
            {
                Assert.Equal(expectedValue, templateInfo.HasSubject);
            }
        }

        [Theory]
        [InlineData("Test1", true)]
        [InlineData("Test2", false)]
        [InlineData("Test3", true)]
        public async Task TestHasHtml(string templateName, bool expectedValue)
        {
            await _initialize;

            foreach (var templateInfo in await _templateManager.GetTemplatesAsync(templateName))
            {
                Assert.Equal(expectedValue, templateInfo.HasHtml);
            }
        }

        [Theory]
        [InlineData("Test1", false)]
        [InlineData("Test2", true)]
        [InlineData("Test3", false)]
        public async Task TestHasText(string templateName, bool expectedValue)
        {
            await _initialize;

            foreach (var templateInfo in await _templateManager.GetTemplatesAsync(templateName))
            {
                Assert.Equal(expectedValue, templateInfo.HasText);
            }
        }
    }
}
