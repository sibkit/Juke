using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BoardFlow.Formats.Common.Reading;

public abstract class CommandsFileReader<T, TC, TP> 
    where T : Enum 
    where TC: ReadingContext, new()
    where TP: new() {
    private readonly Dictionary<T, ICommandReader<T,TC,TP>> _handlers;
    private T[] _nextLikelyTypes;

    protected CommandsFileReader(Dictionary<T, ICommandReader<T,TC,TP>> handlers, T[] nextLikelyTypes) {
        _nextLikelyTypes = nextLikelyTypes;
        _handlers = handlers;
    }

    protected abstract IEnumerable<string> ExcludeCommands(TextReader reader);
    
    public  (TP, TC) Read(FileInfo file) {
        var program = new TP();
        using var streamReader = new StreamReader(
            file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8);
        

        List<string> commands = [];
        commands.AddRange(ExcludeCommands(streamReader));


        var ctx = new TC();
        ctx.Init(commands);
        foreach (var unused in commands) {
            HandleRow(ctx, program);
            if(!ctx.ContinueHandle) break;
            ctx.CurIndex++;
        }
        return (program, ctx);
    }

    
    
    private bool MatchAndHandle(ICommandReader<T, TC, TP> reader, TC ctx, TP program) {
        if (!reader.Match(ctx)) return false;
        reader.WriteToProgram(ctx, program);
        _nextLikelyTypes = reader.GetNextLikelyTypes();
        return true;
    }

    private uint _counter;
    
    private void HandleRow(TC ctx, TP program) {
        if (ctx.CurLine == "") return;
        foreach (var lt in _nextLikelyTypes) {
            if (!_handlers.TryGetValue(lt, out var handler)) {
                throw new Exception("No handler registered for type: \"" + lt + "\"");
            }
            if (MatchAndHandle(handler, ctx, program)) {
                return;
            }
        }

        if (_handlers.Any(entry => MatchAndHandle(entry.Value, ctx, program))) {
            return;
        }

        _counter++;
        ctx.WriteError(_counter.ToString("D4") + " Не найден обработчик для строки: \"" + ctx.CurLine + "\"");
    }
}