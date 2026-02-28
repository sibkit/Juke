using System;
using System.Text.RegularExpressions;
using BoardFlow.Formats.Common;
using BoardFlow.Formats.Common.Entities;
using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Gerber.Entities;

namespace BoardFlow.Formats.Gerber.Reading.CommandReaders;

public partial class FormatSpecificationCommandReader: ICommandReader<GerberCommandType, GerberReadingContext, GerberDocument> {
    private readonly Regex _regex = MatchRegex();
    
    public GerberCommandType[] GetNextLikelyTypes() {
        return [];
    }
    public bool Match(GerberReadingContext ctx) {
        return _regex.IsMatch(ctx.CurLine);
    }
    public void WriteToProgram(GerberReadingContext ctx, GerberDocument document) {
        var m = _regex.Match(ctx.CurLine);
        var z = m.Groups[1].Value switch {
            "L" => Zeros.Leading,
            "T" => Zeros.Trailing,
            "" => Zeros.All,
            _ => throw new Exception("Invalid format specification")
        };

        ctx.CoordinatesMode = m.Groups[2].Value switch {
            "A" => CoordinatesMode.Absolute,
            "I" => CoordinatesMode.Incremental,
            _ => throw new Exception("Invalid coordinates mode")
        };

        if (m.Groups[3].Value != m.Groups[4].Value && m.Groups[3].Value.Length != 2) {
            ctx.WriteError("Не удалось определить спецификацию формата: \""+ctx.CurLine+"\"");
        }
        var l = int.Parse(m.Groups[3].Value[..1]);
        var r = int.Parse(m.Groups[3].Value[1..]);

        ctx.NumberFormat = new NumberFormat {
            Left = l,
            Right = r,
            Zeros = z
        };
    }

    [GeneratedRegex("^FS([LT]{0,1})([AI]{0,1})X([0-9]{2})Y([0-9]{2})\\*$")]
    private static partial Regex MatchRegex();
}