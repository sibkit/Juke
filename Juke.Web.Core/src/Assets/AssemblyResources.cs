using System.Collections.Concurrent;
using System.Reflection;
using Juke.Web.Core.Render;

namespace Juke.Web.Core.Assets;

public class AssemblyResources {
    
    public static IContent LoadContent(string relativePath, Assembly assembly) {
        var resourceName = $"{assembly.GetName().Name}.static.{relativePath.Replace('/', '.')}";
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null) throw new FileNotFoundException($"Resource '{resourceName}' not found!");

        var ext = Path.GetExtension(relativePath).ToLowerInvariant();

        if (IsStringAsset(ext, out var stringType)) {
            using var reader = new StreamReader(stream);
            return new StringContent(stringType, reader.ReadToEnd());
        }

        if (IsBinaryAsset(ext, out var binaryType)) {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return new BinaryContent(binaryType, ms.ToArray());
        }

        throw new NotSupportedException($"Unsupported extension: {ext}");
    }

    public static string ReadRawString(string relativePath, Assembly assembly) {
        var content = LoadContent(relativePath, assembly);
        if (content is not StringContent str) throw new InvalidOperationException("Not a text asset.");
        return str.Text;
    }

    public static InlineAsset Inline(string id, InlinePosition position, string resourcePath, Assembly assembly) {
        var content = LoadContent(resourcePath, assembly);
        if (content is not StringContent stringContent)
            throw new InvalidOperationException("Expected text.");

        return new InlineAsset(id, position, stringContent);
    }

    public static ExternalAsset External(string relativeUrl, string resourcePath, Assembly assembly) {
        return new ExternalAsset(relativeUrl, LoadContent(resourcePath, assembly));
    }

    private static bool IsStringAsset(string ext, out StringContentType type) {
        type = ext switch {
            ".js" => StringContentType.Js, ".css" => StringContentType.Css,
            ".html" => StringContentType.Html, _ => StringContentType.Other
        };
        return ext is ".js" or ".css" or ".html";
    }

    private static bool IsBinaryAsset(string ext, out BinaryContentType type) {
        type = ext switch {
            ".png" or ".webp" or ".jpg" => BinaryContentType.Image,
            ".woff2" or ".ttf" => BinaryContentType.Font, _ => BinaryContentType.Other
        };
        return ext is ".png" or ".webp" or ".jpg" or ".woff2" or ".ttf";
    }
    
    private static readonly ConcurrentDictionary<string, ParsedTemplate> _templateCache = new(StringComparer.OrdinalIgnoreCase);

    public static ParsedTemplate GetTemplate(string relativePath, Assembly assembly)
    {
        // Выполняем парсинг ровно один раз!
        return _templateCache.GetOrAdd(relativePath, path => 
        {
            var text = ReadRawString(path, assembly);
            return ParseTemplate(text);
        });
    }

    private static ParsedTemplate ParseTemplate(string text)
    {
        var parts = new List<IFilePart>();
        int currentIndex = 0;

        while (currentIndex < text.Length)
        {
            int openBraces = text.IndexOf("{{", currentIndex, StringComparison.Ordinal);
            if (openBraces == -1)
            {
                // Параметров больше нет, добавляем остаток как текст
                parts.Add(new StringPart(text.Substring(currentIndex)));
                break;
            }

            // Добавляем текст ДО скобок
            if (openBraces > currentIndex)
            {
                parts.Add(new StringPart(text.Substring(currentIndex, openBraces - currentIndex)));
            }

            int closeBraces = text.IndexOf("}}", openBraces + 2, StringComparison.Ordinal);
            if (closeBraces == -1)
            {
                parts.Add(new StringPart(text.Substring(openBraces)));
                break;
            }

            // Извлекаем имя параметра (без скобок)
            string paramName = text.Substring(openBraces + 2, closeBraces - openBraces - 2).Trim();
            parts.Add(new ParameterPart(paramName));

            currentIndex = closeBraces + 2;
        }

        return new ParsedTemplate(parts);
    }
}