using System;

namespace BoardFlow.Formats.Common.Reading;

public interface ICommandReader<out T, in TC, in TP> 
    where T : Enum 
    where TC: ReadingContext {
    T[] GetNextLikelyTypes();
    bool Match(TC ctx);
    void WriteToProgram(TC ctx, TP layer);
}