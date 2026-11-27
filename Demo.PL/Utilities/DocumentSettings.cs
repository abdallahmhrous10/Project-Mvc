
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Demo.PL.Utilities
{
    public static class DocumentSettings
    {
        public static async Task<string> UploadFileAsync(
            IFormFile file,
            string folderName,
            string webRootPath)
        {
            if (file is null || file.Length == 0)
                throw new ArgumentException("Invalid image file.");

            if (string.IsNullOrWhiteSpace(webRootPath))
                throw new InvalidOperationException(
                    "WebRootPath is not configured.");

            string extension =
                Path.GetExtension(file.FileName).ToLowerInvariant();

            string folderPath = Path.Combine(
                webRootPath,
                "Files",
                folderName
            );

            Directory.CreateDirectory(folderPath);

            string fileName = $"{Guid.NewGuid()}{extension}";

            string filePath = Path.Combine(folderPath, fileName);

            await using var stream = new FileStream(
                filePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                81920,
                FileOptions.Asynchronous
            );

            await file.CopyToAsync(stream);

            return fileName;
        }

        public static void DeleteFile(
            string? fileName,
            string folderName,
            string webRootPath)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            if (string.IsNullOrWhiteSpace(webRootPath))
                throw new InvalidOperationException(
                    "WebRootPath is not configured.");

            string safeFileName = Path.GetFileName(fileName);

            string filePath = Path.Combine(
                webRootPath,
                "Files",
                folderName,
                safeFileName
            );

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
