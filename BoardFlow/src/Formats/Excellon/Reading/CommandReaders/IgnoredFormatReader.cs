using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Excellon.Entities;

namespace BoardFlow.Formats.Excellon.Reading.CommandReaders;

public class IgnoredFormatReader: ICommandReader<ExcellonCommandType, ExcellonReadingContext, Entities.ExcellonDocument> {

    public ExcellonCommandType[] GetNextLikelyTypes() {
        return [];
    }
    public bool Match(ExcellonReadingContext ctx) {
        return ctx.CurLine switch {
            "R,T" => true, //Reset Tool Data
            "M48" => true, //Beginning of a Part Program Header
            "ATC" => true, //Automatic Tool Change
            "ATC,ON" => true,//Automatic Tool Change
            //"M30" => true, //End of Program, rewind   
            "M00" => true, //End of Program, no rewind
            "VER,1" => true, //Use Version 1 X and Y axis layout
            "FMAT,2" => true, //Default FMAT?
            "DETECT,ON" => true, //Broken Tool Detection
            "G93X0Y0" => true, //Sets work zero relative to absolute zero
            "TCST,OFF" => true, //Tool Change Stop Switch
            "G92" => true,
            _ => false
        };
    }
    public void WriteToProgram(ExcellonReadingContext ctx, Entities.ExcellonDocument document) {
        ctx.WriteInfo("Пропущенная команда: "+ctx.CurLine);
    }
}