using BoardFlow.Formats.Gerber.Entities.Apertures.Macro.Expressions;

namespace BoardFlow.Formats.Gerber.Reading.Macro.Tokenize;


public interface IToken {
    int SourceIndex { get; set; }
};

public struct ParensOpenToken: IToken {
    public int SourceIndex { get; set; }
}

public struct ParensCloseToken: IToken {
    public int SourceIndex { get; set; }
}

public struct ValueToken(double value) : IToken {
    public double Value { get; init; } = value;
    public int SourceIndex { get; set; }
}

public struct ParameterToken(string name) : IToken {
    public string Name { get; init; } = name;
    public int SourceIndex { get; set; }
}

public struct OperationToken(OperationType type) : IToken {
    public OperationType Type { get; init; } = type;
    public int SourceIndex { get; set; }
}

