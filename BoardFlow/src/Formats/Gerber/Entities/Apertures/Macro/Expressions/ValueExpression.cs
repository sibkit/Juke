using System;

namespace BoardFlow.Formats.Gerber.Entities.Apertures.Macro.Expressions;

public class ValueExpression: IExpression {
    public ValueExpression(double value) {
        Value = value;
    }
    public double Value { get; set; }

    public override string ToString() {
        return Math.Round(Value, 6).ToString();
    }
}