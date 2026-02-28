using System;
using System.Text.RegularExpressions;
using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Gerber.Entities;
using BoardFlow.Formats.Gerber.Entities.Apertures.Macro;
using BoardFlow.Formats.Gerber.Entities.Apertures.Macro.Primitives;
using BoardFlow.Formats.Gerber.Reading.Macro;

namespace BoardFlow.Formats.Gerber.Reading.CommandReaders;

public partial class DefineApertureTemplateCommandReader: ICommandReader<GerberCommandType, GerberReadingContext, GerberDocument>  {
    
    [GeneratedRegex(@"^%AM.+%$")]
    private static partial Regex MatchRegex();
    
    public GerberCommandType[] GetNextLikelyTypes() {
        return [];
    }
    public bool Match(GerberReadingContext ctx) {
        return MatchRegex().IsMatch(ctx.CurLine);
    }

    // private static ParameterExpression ReadParameter(string line) {
    //     var parts = line.Split("=", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    //     if (parts.Length != 2) {
    //         throw new ApplicationException("Invalid parameter format");
    //     }
    //     var result = new ParameterExpression(parts[0]);
    //     
    //     return result;
    // }

    public static IPrimitive ReadPrimitive(string line) {
        var lineParts = line.Split(',');
        return lineParts[0] switch {
            "1" => ReadCircle(lineParts),
            "4" => ReadOutline(lineParts),
            "5" => ReadPolygon(lineParts),
            "7" => ReadThermal(lineParts),
            "20" => ReadVectorLine(lineParts),
            "21" => ReadCenterLine(lineParts),
            _ => throw new ApplicationException("Invalid primitive code")
        };
    }
    
    public void WriteToProgram(GerberReadingContext ctx, GerberDocument document) {
        var lines = ctx.CurLine.Trim('%').Split('*', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var name = lines[0][2..];
        var am = new MacroApertureTemplate(name);
        foreach (var line in lines[1..]) {
            if (line.StartsWith('$')) {
                var parts = line.Split('=');
                am.Expressions.Add(parts[0], ReadExpression(parts[1]));
            } else if(line.StartsWith('0')){
                am.Comments.Add(line);
            } else {
                am.Primitives.Add(ReadPrimitive(line));
            }
        }
        //Console.WriteLine(ctx.CurLine);
        document.MacroApertureTemplates.Add(name, am);
    }

    private static IExpression ReadExpression(string expression) {
        return new ExpressionBuilder(expression).Build();
    }

    private static CenterLine ReadCenterLine(string[] values) {
        var result = new CenterLine {
            Exposure = ReadExpression(values[1]),
            Width = ReadExpression(values[2]),
            Height = ReadExpression(values[3]),
            CenterX = ReadExpression(values[4]),
            CenterY = ReadExpression(values[5]),
            Rotation =  ReadExpression(values[6])
        };
        return result;
    }

    private static Polygon ReadPolygon(string[] values) {
        var result = new Polygon {
            Exposure = ReadExpression(values[1]),
            VerticesCount = ReadExpression(values[2]),
            CenterX = ReadExpression(values[3]),
            CenterY = ReadExpression(values[4]),
            Diameter = ReadExpression(values[5]),
            Rotation =  ReadExpression(values[6])
        };
        return result;
    }
    
    private static Outline ReadOutline(string[] values) {
        var result = new Outline {
            Exposure = ReadExpression(values[1]),
        };
        var verticesCount = int.Parse(values[2]);
        for (var i = 0; i < verticesCount; i++) {
            result.Vertices.Add((
                ReadExpression(values[3+i*2]),
                ReadExpression(values[4+i*2])
                ));
        }
        result.Rotation = ReadExpression(values[5+2*verticesCount]);
        return result;
    }

    private static Thermal ReadThermal(string[] values) {
        return new Thermal {
            CenterX = ReadExpression(values[1]),
            CenterY = ReadExpression(values[2]),
            OuterDiameter = ReadExpression(values[3]),
            InnerDiameter = ReadExpression(values[4]),
            GapThickness = ReadExpression(values[5]),
            Rotation = ReadExpression(values[6])
        };
    }

    private static Circle ReadCircle(string[] values) {

        var result = new Circle {
            Exposure = ReadExpression(values[1]),
            Diameter = ReadExpression(values[2]),
            CenterX = ReadExpression(values[3]),
            CenterY = ReadExpression(values[4]),
            //Rotation = ReadExpression(values[5])
        };
        return result;
    }
    
    private static VectorLine ReadVectorLine(string[] values) {

        var result = new VectorLine {
            Exposure = ReadExpression(values[1]),
            Width = ReadExpression(values[2]),
            StartX = ReadExpression(values[3]),
            StartY = ReadExpression(values[4]),
            EndX = ReadExpression(values[5]),
            EndY = ReadExpression(values[6]),
            Rotation = ReadExpression(values[7])
        };
        return result;
    }
}