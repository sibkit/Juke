using System.Collections.Generic;

namespace BoardFlow.Formats.Gerber.Reading.Macro.Syntax;

public class SyntaxExpression {
    public List<ISyntaxExpressionPart> Parts { get; } = [];
}