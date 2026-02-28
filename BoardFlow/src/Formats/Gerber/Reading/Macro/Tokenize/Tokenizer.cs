using System;
using System.Collections.Generic;
using System.Globalization;
using BoardFlow.Formats.Gerber.Entities.Apertures.Macro.Expressions;

namespace BoardFlow.Formats.Gerber.Reading.Macro.Tokenize;

public static class Formatter {
    public static readonly IFormatProvider FormatProvider = new NumberFormatInfo { NumberDecimalSeparator = "." };
}

public interface ITokenReader {
    bool IsMatch(string text, int index);
    (IToken?, int) Read(string text, int index);
}

public class ParensOpenTokenReader : ITokenReader {
    public bool IsMatch(string text, int index) {
        return text[index] == '(';
    }
    public (IToken?, int) Read(string text, int index) {
        return (new ParensOpenToken(), 1);
    }
}

public class ParensCloseTokenReader : ITokenReader {
    public bool IsMatch(string text, int index) {
        return text[index] == ')';
    }
    public (IToken?, int) Read(string text, int index) {
        return (new ParensCloseToken(), 1);
    }
}

public class ValueTokenReader : ITokenReader {
    public bool IsMatch(string text, int index) {
        return char.IsDigit(text,index) || text[index] == '.';
    }
    public (IToken?, int) Read(string text, int index) {
        var resultString = "";
        for (var i = index; i < text.Length; i++) {
            if(char.IsDigit(text,i) || text[i] == '.')
                resultString += text[i];
            else break;
        }
        return (new ValueToken(double.Parse(resultString, Formatter.FormatProvider)), resultString.Length);
    }
}

public class IgnoringTokenReader : ITokenReader {
    public bool IsMatch(string text, int index) {
        return text[index] == ' ';
    }
    public (IToken?, int) Read(string text, int index) {
        return (null, 1);
    }
}

public class ParameterTokenReader : ITokenReader {
    public bool IsMatch(string text, int index) {
        return text[index] == '$';
    }
    public (IToken?, int) Read(string text, int index) {
        var resultString = "$";
        for (var i = index+1; i < text.Length; i++) {
            if(char.IsDigit(text,i))
                resultString += text[i];
            else break;
        }
        return (new ParameterToken(resultString), resultString.Length);
    }
}

public class OperationTokenReader : ITokenReader {
    public bool IsMatch(string text, int index) {
        return "+-x/*".Contains(text[index]);
    }
    public (IToken?, int) Read(string text, int index) {
        var operationType = text[index] switch {
            '+' => OperationType.Add,
            '-' => OperationType.Subtract,
            'x' or '*' => OperationType.Multiply,
            '/' => OperationType.Divide,
            _ => throw new Exception("Invalid operation")
        };
        return (new OperationToken(operationType), 1);
    }
}

public class Tokenizer {
    public Tokenizer() {
        Readers.Add(new ParensOpenTokenReader());
        Readers.Add(new ParensCloseTokenReader());
        Readers.Add(new ValueTokenReader());
        Readers.Add(new IgnoringTokenReader());
        Readers.Add(new ParameterTokenReader());
        Readers.Add(new OperationTokenReader());
    }
    public List<ITokenReader> Readers { get; init; } = [];
    
    public IList<IToken>? Tokenize(string text) {
        var tokens = new List<IToken>();
        for (var i = 0; i < text.Length; ) {
            foreach (var reader in Readers) {
                if (!reader.IsMatch(text, i)) continue;
                var rr = reader.Read(text, i);
                if (rr.Item1 != null) {
                    rr.Item1.SourceIndex = i;
                    tokens.Add(rr.Item1);
                }
                i += rr.Item2;
                goto NEXT_CHAR;
            }
            throw new Exception("Invalid token");
            NEXT_CHAR: ;
        }
        return tokens;
    }
}