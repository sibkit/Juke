using System;
using System.Collections.Generic;
using BoardFlow.Formats.Gerber.Entities.Apertures.Macro.Expressions;
using BoardFlow.Formats.Gerber.Reading.Macro.Syntax;
using BoardFlow.Formats.Gerber.Reading.Macro.Tokenize;

namespace BoardFlow.Formats.Gerber.Reading.Macro;


public class SyntaxExpressionBuilder {
    private (SyntaxExpression, int) BuildExpression(int index) {

        switch (_tokens![index]) {
            case OperationToken op:
                switch (op.Type) {
                    case OperationType.Subtract:
                        _tokens.Insert(index, new ValueToken(-1));
                        _tokens[index + 1] = new OperationToken(OperationType.Multiply);
                        break;
                    case OperationType.Add:
                        _tokens.RemoveAt(index);
                        break;
                    case OperationType.Multiply:
                    case OperationType.Divide:
                    default:
                        throw new ParseExpressionException("Ошибка разбора строки", _tokens[index].SourceIndex);
                }
                break;
        }
        
        var isOdd = true;
        var shift = 0;
        var result = new SyntaxExpression();
        for (var i = index; i < _tokens!.Count; i++) {
            switch (_tokens[i]) {
                case ParensOpenToken:
                    if (!isOdd)
                        throw new ParseExpressionException("Ожидается оператор - обнаружена открывающая скобка", _tokens[i].SourceIndex);
                    var group = new SyntaxGroup();
                    var buildResult = BuildExpression(i + 1);
                    group.Expression = buildResult.Item1;
                    i += buildResult.Item2 + 1;
                    shift += buildResult.Item2 + 1;

                    if (_tokens[i] is not ParensCloseToken) {
                        throw new ParseExpressionException("Ожидается закрывающая скобка", _tokens[i].SourceIndex);
                    }

                    result.Parts.Add(group);
                    break;
                case ParensCloseToken:
                    if (index == 0) throw new ParseExpressionException("Ожидается операнд - обнаружена закрывающая скобка", _tokens[i].SourceIndex);
                    return (result, shift);
                case OperationToken ot:
                    if (isOdd) throw new ParseExpressionException("Ожидается операнд - обнаружен оператор", _tokens[i].SourceIndex);
                    var sop = new SyntaxOperator(ot.Type);
                    result.Parts.Add(sop);
                    break;
                case ValueToken vt:
                    if (!isOdd) throw new ParseExpressionException("Ожидается оператор - обнаружено значение", _tokens[i].SourceIndex);
                    result.Parts.Add(new SyntaxValue(vt.Value));
                    break;
                case ParameterToken pt:
                    if (!isOdd) throw new ParseExpressionException("Ожидается оператор - обнаружен параметр", _tokens[i].SourceIndex);
                    result.Parts.Add(new SyntaxParameter(pt.Name));
                    break;
                default:
                    throw new Exception("SyntaxExpressionBuilder: BuildExpression (Unexpected token type)");
            }
            shift++;
            isOdd = !isOdd;
        }

        if (index != 0)
            throw new ParseExpressionException("Некоректное завершение выражения", index);
        return (result, shift);
    }

    private IList<IToken>? _tokens;

    public SyntaxExpression Build(string expression) {
        _tokens = new Tokenizer().Tokenize(expression);
        if (_tokens == null || _tokens!.Count == 0) {
            throw new ParseExpressionException("Обнаружена пустая строка", -1);
        }

        var result = BuildExpression(0);
        return result.Item1;
    }
}