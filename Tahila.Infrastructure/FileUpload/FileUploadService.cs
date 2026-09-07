using Microsoft.AspNetCore.Http;

namespace Tahila.Infrastructure.FileUpload;

public class FileUploadService
{
    private readonly string _uploadRoot;
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
    private static readonly string[] AllowedVideoExtensions = [".mp4", ".webm"];

    public FileUploadService(string uploadRoot) => _uploadRoot = uploadRoot;

    public async Task<string> UploadImageAsync(IFormFile file, string folder)
    {
        ValidateExtension(file.FileName, AllowedImageExtensions);
        return await SaveFileAsync(file, folder);
    }

    public async Task<string> UploadVideoAsync(IFormFile file, string folder)
    {
        ValidateExtension(file.FileName, AllowedVideoExtensions);
        return await SaveFileAsync(file, folder);
    }

    public void DeleteFile(string relativePath)
    {
        var fullPath = Path.Combine(_uploadRoot, relativePath.TrimStart('/'));
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }

    private async Task<string> SaveFileAsync(IFormFile file, string folder)
    {
        var dir = Path.Combine(_uploadRoot, folder);
        Directory.CreateDirectory(dir);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(dir, fileName);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/{folder}/{fileName}";
    }

    private static void ValidateExtension(string fileName, string[] allowed)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (!allowed.Contains(ext))
            throw new InvalidOperationException($"فرمت فایل مجاز نیست. فرمت‌های مجاز: {string.Join(", ", allowed)}");
    }
}
