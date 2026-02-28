using System;
using System.Collections.Generic;

namespace BoardFlow.Formats.Common.Reading;

public abstract class ReadingContext {
    private List<string>? _lines;
    public bool ContinueHandle { get; set; } = true;
    
    
    internal void Init(List<string> lines) {
        _lines = lines;
    }
    
    public int CurIndex { get; set; } = 0;

    public List<string> Lines => _lines ?? throw new Exception("Context: Not initialized");

    public string CurLine => Lines[CurIndex];
    
    public List<string> Warnings { get; } = [];
    public List<string> Infos { get; } = [];
    public List<string> Errors { get; } = [];
    
    public void WriteInfo(string info) {
        Infos.Add(info);
    }
    public void WriteError(string error) {
        Errors.Add(error);
    }
    public void WriteWarning(string warning) {
        Warnings.Add(warning);
    }
}