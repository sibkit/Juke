using System.Collections.Generic;
using BoardFlow.Formats.Gerber.Entities.Apertures;
using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Formats.Gerber.Entities;

public class PathPaintOperation: IPaintOperation {
    public PathPaintOperation(CircleAperture aperture, Point startPoint) {
        Aperture = aperture;
        StartPoint = startPoint;
    }
    public Point StartPoint { get; set; }
    public CircleAperture Aperture { get; set; }
    public List<IPathPart> Parts { get; } = [];
    public bool IsClosed { get; set; } = false;
}
