using System;
using System.Globalization;
using System.Text.RegularExpressions;
using BoardFlow.Formats.Common;
using BoardFlow.Formats.Common.Entities;
using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Formats.Excellon.Reading;

internal class StringCoordinate {
    public string? X{get;set;}
    public string? Y{get;set;}
}

public static partial class ExcellonCoordinates {
    
    private static readonly Regex ReCoordinates = MyRegex();
    private static readonly IFormatProvider Formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
    
    private static bool FillCoordinate(string? axe, string? val, StringCoordinate coordinate) {
        if (string.IsNullOrEmpty(axe) || string.IsNullOrEmpty(val))
            return false;
            
        switch (axe.ToUpperInvariant()) {
            case "X":
                coordinate.X = val;
                return true;
            case "Y":
                coordinate.Y = val;
                return true;
        }
        return false;
    }
    
    private static StringCoordinate? ReadStringCoordinate(string line) {
        var match = ReCoordinates.Match(line);
        if (!match.Success) return null;
        var sc = new StringCoordinate();
        
        // ИСПРАВЛЕНИЕ 1: Убираем ленивое вычисление (short-circuit), чтобы Y всегда парсился!
        var f1 = FillCoordinate(match.Groups[1].Value, match.Groups[2].Value, sc);
        var f2 = FillCoordinate(match.Groups[3].Value, match.Groups[4].Value, sc);
        
        return f1 || f2 ? sc : null;
    }
    

    private static double ReadValue(string value, ExcellonReadingContext ctx) {
        
        if (value.Contains('.')) {
            ctx.CoordinatesDefineState.AccurateValueDetected = true;
            return double.Parse(value, Formatter);
        } 

        ctx.CoordinatesDefineState.CalculateValueDetected = true;
        
        if (ctx.NumberFormat is { Left: not null, Right: not null } &&
            value.Length != ctx.NumberFormat.Left + ctx.NumberFormat.Right) {
            ctx.CoordinatesDefineState.DifferentLineLengthsDetected = true;
        }
        
        if (ctx.NumberFormat.Zeros != null && (ctx.NumberFormat.Left != null || ctx.NumberFormat.Right != null)) {
            return Coordinates.ReadValue(ctx.NumberFormat, value);
        }

        if (ctx.NumberFormat.Zeros != null) {
            ctx.CoordinatesDefineState.UndefinedScaleDetected = true;
            // ИСПРАВЛЕНИЕ: Дюймы чаще всего имеют формат 2.4, а метрика 4.2
            return Coordinates.ReadValue(new NumberFormat {
                Left = ctx.Uom == Uom.Inch ? 2 : 4,
                Right = ctx.Uom == Uom.Inch ? 4 : 2,
                Zeros = ctx.NumberFormat.Zeros
            }, value);
        }

        if (ctx.NumberFormat is {Zeros: null, Left: not null, Right: not null }) {
            if(value.Trim('+').Trim('-').Length == ctx.NumberFormat.Left + ctx.NumberFormat.Right)
                return Coordinates.ReadValue(new NumberFormat {
                    Left = ctx.NumberFormat.Left,
                    Right = ctx.NumberFormat.Right,
                    Zeros = Zeros.All
                }, value);
            ctx.WriteError("Не удалось определить значение координаты: \"" + value + "\"");
        }

        if (ctx.NumberFormat is { Left: null, Right: null, Zeros: null }) {
            if (value.Length == 6)
                return Coordinates.ReadValue(new NumberFormat {
                    Left = ctx.Uom == Uom.Inch ? 2 : 4,
                    Right = ctx.Uom == Uom.Inch ? 4 : 2,
                    Zeros = Zeros.All
                }, value);
        }
        
        return 0;
    }
    
    public static Point? ReadCoordinate(string line, ExcellonReadingContext ctx) {
        var sc = ReadStringCoordinate(line);
        if (sc == null) 
            return null;
            
        // ИСПРАВЛЕНИЕ 2: Модальные координаты. Если оси X или Y нет в строке, 
        // берем предыдущее значение из контекста (ctx.CurPoint)!
        var x = !string.IsNullOrEmpty(sc.X) ? ReadValue(sc.X, ctx) : ctx.CurPoint.X;
        var y = !string.IsNullOrEmpty(sc.Y) ? ReadValue(sc.Y, ctx) : ctx.CurPoint.Y;
        
        return new Point(x,y);
    }

    public static bool IsCoordinate(string line) {
        return ReCoordinates.IsMatch(line);
    }
    
    [GeneratedRegex("^(?:([XYxy])([+-]?[0-9.]+))?(?:([XYxy])([+-]?[0-9.]+))?$")]
    private static partial Regex MyRegex();
}