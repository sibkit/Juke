using System.Text;

namespace Juke.Web.Core.Render;

public interface IFilePart
{
    void AppendTo(StringBuilder builder, Func<string, string> parameterResolver);
}

public class StringPart : IFilePart
{
    private readonly string _text;
    public StringPart(string text) => _text = text;

    public void AppendTo(StringBuilder builder, Func<string, string> parameterResolver)
    {
        builder.Append(_text); 
    }
}

public class ParameterPart : IFilePart {
    private readonly string _parameterName;
    public ParameterPart(string parameterName) => _parameterName = parameterName;

    public void AppendTo(StringBuilder builder, Func<string, string> parameterResolver) {
        builder.Append(parameterResolver(_parameterName));
    }
}

public class ParsedTemplate {
    private readonly IReadOnlyList<IFilePart> _parts;
    public ParsedTemplate(IReadOnlyList<IFilePart> parts) => _parts = parts;

    public void RenderTo(StringBuilder builder, Func<string, string> parameterResolver) {
        foreach (var part in _parts) {
            part.AppendTo(builder, parameterResolver);
        }
    }
}