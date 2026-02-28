using BoardFlow.Formats.Gerber.Reading.Macro.Tokenize;

namespace BoardFlow.Formats.Gerber.Reading.Macro.Syntax;

public class SyntaxGroup : ISyntaxOperand {
    public SyntaxExpression? Expression { get; set; }
    public IToken? Token { get; set; }
}