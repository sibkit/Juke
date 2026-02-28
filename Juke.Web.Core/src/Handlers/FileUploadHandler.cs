/* Juke.Web.Core/Handlers/FileUploadHandler.cs */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Juke.Web.Core.Http;

namespace Juke.Web.Core.Handlers;

// DTO для передачи информации о сохраненном файле в бизнес-логику
public record SavedFileInfo(string OriginalFileName, string PhysicalPath, long Length);

public abstract class FileUploadHandler : IRequestHandler
{
    // --- Контракты для бизнес-модулей ---
    
    // Модуль должен указать, в какую папку сохранять файлы
    protected abstract string GetTargetDirectory();
    
    // Модуль может ограничить расширения (например, [".zip"])
    protected virtual string[] AllowedExtensions => []; 
    
    // Вызывается, когда ядро успешно сохранило все файлы на диск
    protected abstract Task OnFilesSavedAsync(IHttpContext context, IReadOnlyList<SavedFileInfo> savedFiles);

    // --- Реализация инфраструктуры (Ядро) ---
    
    public async Task HandleAsync(IHttpContext context)
    {
        // 1. Проверки протокола
        if (context.Request.Method != Method.POST || !context.Request.HasFormContentType)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Bad Request: Expected multipart/form-data.");
            return;
        }

        var files = context.Request.Files.ToList();
        if (files.Count == 0)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Bad Request: No files uploaded.");
            return;
        }

        var targetDir = GetTargetDirectory();
        Directory.CreateDirectory(targetDir);

        var allowedExts = AllowedExtensions;
        var savedFiles = new List<SavedFileInfo>();

        // 2. Безопасное сохранение
        foreach (var file in files)
        {
            if (file.Length == 0) continue;

            // Проверка расширения (если заданы ограничения)
            var ext = Path.GetExtension(file.FileName);
            if (allowedExts.Length > 0 && !allowedExts.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync($"Bad Request: Extension '{ext}' is not allowed.");
                return;
            }

            // Генерация безопасного имени
            var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var savePath = Path.Combine(targetDir, safeFileName);

            // Zero-Allocation потоковое копирование
            await using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await using var uploadedStream = file.OpenReadStream();
                await uploadedStream.CopyToAsync(fileStream);
            }

            savedFiles.Add(new SavedFileInfo(file.FileName, savePath, file.Length));
        }

        if (savedFiles.Count == 0)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Bad Request: Uploaded files were empty or invalid.");
            return;
        }

        // 3. Передача управления в бизнес-модуль
        await OnFilesSavedAsync(context, savedFiles);
    }
}