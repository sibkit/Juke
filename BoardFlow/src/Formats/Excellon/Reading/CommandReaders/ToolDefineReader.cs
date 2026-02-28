using System;
using System.Globalization;
using System.Text.RegularExpressions;
using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Excellon.Entities;

namespace BoardFlow.Formats.Excellon.Reading.CommandReaders;

public partial class ToolDefineReader: ICommandReader<ExcellonCommandType, ExcellonReadingContext, Entities.ExcellonDocument> {
    
    private static readonly Regex ReToolDefine = ToolDefineRegex();
    private readonly IFormatProvider _formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
    public ExcellonCommandType[] GetNextLikelyTypes() {
        return[ExcellonCommandType.ToolDefine, ExcellonCommandType.EndHeader];
    }
    public bool Match(ExcellonReadingContext ctx) {
        return ReToolDefine.IsMatch(ctx.CurLine);
    }
    public void WriteToProgram(ExcellonReadingContext ctx, Entities.ExcellonDocument document) {
        var match = ReToolDefine.Match(ctx.CurLine);
        if (match.Groups.Count == 3) {
            var toolNum = int.Parse(match.Groups[1].Value);
            document.ToolsMap.Add(toolNum, decimal.Parse(match.Groups[2].Value, _formatter));
        } else {
            throw new Exception("ToolDefineHandler.WriteToProgram: Invalid line.");
        }
    }
    
    [GeneratedRegex(@"T([0-9]+)[A-Z0-9]*C([0-9.]+)(?:[A-Z]*|$)")]
    private static partial Regex ToolDefineRegex();
}