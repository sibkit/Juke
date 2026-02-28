using System.Collections.Generic;
using BoardFlow.Formats.Common;
using BoardFlow.Formats.Common.Entities;

namespace BoardFlow.Formats.Excellon.Entities;

public class ExcellonDocument
{
    public List<IMachiningOperation> Operations { get; } = [];
    public Uom? Uom {get; set;} = null;
    public Dictionary<int,decimal> ToolsMap { get; } = new Dictionary<int, decimal>();
}

