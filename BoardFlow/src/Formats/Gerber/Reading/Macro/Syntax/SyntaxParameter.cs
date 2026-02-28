using BoardFlow.Formats.Gerber.Reading.Macro.Tokenize;

namespace BoardFlow.Formats.Gerber.Reading.Macro.Syntax;

public class SyntaxParameter(string name): ISyntaxOperand {
    public string Name { get; } = name;
    public IToken? Token { get; set; }
}