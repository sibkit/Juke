/* SgmToSvgConverter.cs */
using System;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;
using BoardFlow.Formats.Svg.Entities;
using BoardFlow.Formats.Svg.Writing;

namespace BoardFlow.Converters.SgmToSvg;

public static class SgmToSvgConverter {
    public static SvgDocument Convert(SgmDocument sgmDocument) {
        var result = new SvgDocument {
            ViewBox = sgmDocument.Bounds,
            Uom = sgmDocument.Uom // Передаем Uom
        };
        result.Elements.AddRange(sgmDocument.GraphicElements);
        
        return result;
    }

    public static void WriteContour(Contour contour, string filename) {
        var area = new SgmDocument();
        area.GraphicElements.Add(contour);
        var layer = Convert(area);
        SvgWriter.Write(layer, filename);
    }
}