using BoardFlow.Formats.Gerber.Reading.Macro.Tokenize;

namespace BoardFlow.Formats.Gerber.Reading.Macro.Syntax;

public interface ISyntaxExpressionPart {
    public IToken Token { get; set; }
}