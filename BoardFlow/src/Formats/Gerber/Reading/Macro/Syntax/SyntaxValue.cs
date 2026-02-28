using BoardFlow.Formats.Gerber.Reading.Macro.Tokenize;

namespace BoardFlow.Formats.Gerber.Reading.Macro.Syntax;

public class SyntaxValue(double value): ISyntaxOperand {
    public double Value { get; } = value;
    public IToken? Token { get; set; }
}