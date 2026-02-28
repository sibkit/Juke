using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Gerber.Entities;
using BoardFlow.Formats.Gerber.Entities.Apertures;
using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Formats.Gerber.Reading.CommandReaders;

public class BeginRegionCommandReader: ICommandReader<GerberCommandType, GerberReadingContext, GerberDocument> {
    public GerberCommandType[] GetNextLikelyTypes() {
        return [];
    }
    public bool Match(GerberReadingContext ctx) {
        return ctx.CurLine == "G36*";
    }
    public void WriteToProgram(GerberReadingContext ctx, GerberDocument document) {
        
        if (ctx.CurApertureCode == null) {
            ctx.WriteError("Не задана аппертура перед командой G36");
            return;
        }
            
        var curAperture = document.Apertures[ctx.CurApertureCode.Value];
        
        switch (curAperture) {
            case CircleAperture ca:
                if (ctx.CurCoordinate == null) {
                    ctx.WriteError("Не задана начальная координата перед командой G36");
                }
                ctx.CurPathPaintOperation ??= new PathPaintOperation(ca, (Point)ctx.CurCoordinate!);
                
                break;
            case null:
                ctx.WriteError("Не задана аппертура перед командой G36");
                break;              
            default:
                ctx.WriteError("Неправильная аппертура: для операции D01 поддерживаются только круговые аппертуры");
                break;
        }
    }
}