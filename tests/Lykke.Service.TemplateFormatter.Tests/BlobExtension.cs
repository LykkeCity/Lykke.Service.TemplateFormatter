using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using JetBrains.Annotations;

namespace Lykke.Service.TemplateFormatter.Tests
{
    public static class BlobExtension
    {
        public static async Task LoadBlobFilesAsync([NotNull] this IBlobStorage blob, [NotNull] string containerName)
        {
            if (blob == null) throw new ArgumentNullException(nameof(blob));
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerName));

            var directory = GetTestDirectory(containerName);
            if (!directory.Exists)
                return;

            await LoadBlobFilesAsync(blob, containerName, directory, string.Empty);
        }

        private static async Task LoadBlobFilesAsync(this IBlobStorage blob, string containerName, DirectoryInfo directory, string path)
        {
            foreach (var file in directory.GetFiles())
            {
                using (var stream = file.OpenRead())
                    await blob.SaveBlobAsync(containerName, Path.Combine(path, file.Name).Replace('\\', '/'), stream);
            }
            foreach (var subdirectory in directory.GetDirectories())
            {
                await blob.LoadBlobFilesAsync(containerName, subdirectory, Path.Combine(path, subdirectory.Name));
            }
        }

        private static DirectoryInfo GetTestDirectory(string directoryName)
        {
            return new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Test_Data", directoryName));
        }

    }
}
