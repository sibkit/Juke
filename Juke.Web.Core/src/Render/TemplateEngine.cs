using System.Text;

namespace Juke.Web.Core.Render;

public interface IFilePart
{
    // Принимает билдер и функцию, которая по имени параметра вернет значение
    void AppendTo(StringBuilder builder, Func<string, string> parameterResolver);
}

// Кусок обычного статического текста
public class StringPart : IFilePart
{
    private readonly string _text;
    public StringPart(string text) => _text = text;

    public void AppendTo(StringBuilder builder, Func<string, string> parameterResolver)
    {
        builder.Append(_text); // Zero-allocation: просто копируем заранее созданную строку
    }
}

// Кусок параметра (например, "content1")
public class ParameterPart : IFilePart
{
    private readonly string _parameterName;
    public ParameterPart(string parameterName) => _parameterName = parameterName;

    public void AppendTo(StringBuilder builder, Func<string, string> parameterResolver)
    {
        // Вызываем анонимную функцию, чтобы получить реальное значение на лету!
        builder.Append(parameterResolver(_parameterName)); 
    }
}

// Готовый скомпилированный шаблон
public class ParsedTemplate
{
    private readonly IReadOnlyList<IFilePart> _parts;
    public ParsedTemplate(IReadOnlyList<IFilePart> parts) => _parts = parts;

    // Сборка шаблона на лету
    public void RenderTo(StringBuilder builder, Func<string, string> parameterResolver)
    {
        foreach (var part in _parts)
        {
            part.AppendTo(builder, parameterResolver);
        }
    }
}