using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Gerber.Entities;
using BoardFlow.Formats.Gerber.Entities.Apertures;

namespace BoardFlow.Formats.Gerber.Reading.CommandReaders;

public partial class DefineApertureCommandReader: ICommandReader<GerberCommandType, GerberReadingContext, GerberDocument>  {
    

    
    [GeneratedRegex(@"^ADD([0-9]+)([A-Za-z]{1}[A-Za-z0-9]*)(?:,{1}(.*))*\*$")]
    private static partial Regex MatchRegex();
    private static readonly IFormatProvider Formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
    

    private static List<double?> ParseParams(string input) {
        var result = new List<double?>();
        foreach (var value in input.Split("X")) {
            if(value == "")
                result.Add(null);
            else
                result.Add(double.Parse(value, Formatter));
        }
        return result;
    }

    private static CircleAperture ParseCircleAperture(string sParams) {
        var prs = ParseParams(sParams);
        return prs.Count switch {
            1 => new CircleAperture {
                Diameter = prs[0]!.Value,
            },
            2 => new CircleAperture {
                Diameter = prs[0]!.Value,
                HoleDiameter = prs[1]
            },
            _ => throw new Exception("DefineApertureHandler::ParseRectangleAperture(): Invalid string")
        };
    }

    private static RectangleAperture ParseRectangleAperture(string sParams) {
        var prs = ParseParams(sParams);
        return prs.Count switch {
            2 => new RectangleAperture {
                XSize = prs[0]!.Value,
                YSize = prs[1]!.Value
            },
            3 => new RectangleAperture {
                XSize = prs[0]!.Value,
                YSize = prs[1]!.Value,
                HoleDiameter = prs[2]
            },
            _ => throw new Exception("DefineApertureHandler::ParseRectangleAperture(): Invalid string")
        };
    }
    
    private static ObRoundAperture ParseObRoundAperture(string sParams) {
        var prs = ParseParams(sParams);
        return prs.Count switch {
            2 => new ObRoundAperture {
                XSize = prs[0]!.Value,
                YSize = prs[1]!.Value,
            },
            3 => new ObRoundAperture {
                XSize = prs[0]!.Value,
                YSize = prs[1]!.Value,
                HoleDiameter = prs[2]
            },
            _ => throw new Exception("DefineApertureHandler::ParseObRoundAperture(): Invalid string")
        };
    }
    
    private static PolygonAperture ParsePolygonAperture(string sParams) {
        var prs = ParseParams(sParams);
        var outerDiameter = prs[0]!.Value;
        var verticesCount = (int)prs[1]!.Value;
        double? rotation = null;
        if (sParams.Length > 2) {
            rotation = prs[2];
        }

        double? holeDiameter = null;
        if (sParams.Length == 4) {
            holeDiameter = prs[3];
        }

        return new PolygonAperture {
            OuterDiameter = outerDiameter,
            VerticesCount = verticesCount,
            Rotation = rotation,
            HoleDiameter = holeDiameter
        };
    }

    private static MacroAperture ParseMacroAperture(string templateName, string sParams) {
        var prs = ParseParams(sParams);
        var result = new MacroAperture(templateName);

        foreach (var pr in prs) {
            if (pr != null)
                result.ParameterValues.Add(pr.Value);
        }
        return result;
    }

    public GerberCommandType[] GetNextLikelyTypes() {
        return [];
    } 
    
    public bool Match(GerberReadingContext ctx) {
        return ctx.CurLine.StartsWith("ADD");
    }
    
    public void WriteToProgram(GerberReadingContext ctx, GerberDocument document) {
        
        // var mm = MatchMacroRegex().Match(ctx.CurLine);
        // if (mm.Success) {
        //     var code = int.Parse(mm.Groups[1].Value);
        //     var templateName = mm.Groups[2].Value;
        //     program.Apertures.Add(code, new MacroAperture(templateName));
        //     return;
        // }
        
        var m = MatchRegex().Match(ctx.CurLine);
        if (m.Success) {
            var appNum = int.Parse(m.Groups[1].Value);
            var sParams = m.Groups[3].Value;
            switch (m.Groups[2].Value) {
                case "C":
                    document.Apertures.Add(appNum, ParseCircleAperture(sParams));
                break;
                case "R":
                    document.Apertures.Add(appNum, ParseRectangleAperture(sParams));
                    break;
                case "O":
                    document.Apertures.Add(appNum, ParseObRoundAperture(sParams));
                    break;
                case "P":
                    document.Apertures.Add(appNum, ParsePolygonAperture(sParams));
                    break;
                default:
                    document.Apertures.Add(appNum, ParseMacroAperture(m.Groups[2].Value, sParams));
                    break;
            }
        } else {
            throw new Exception("DefineApertureHandler: WriteToProgram (Invalid match)");
        }
    }


}