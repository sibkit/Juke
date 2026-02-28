using System;
using System.Collections.Generic;
using System.Linq;
using BoardFlow.Formats.Gerber.Entities.Apertures.Macro;
using BoardFlow.Formats.Gerber.Entities.Apertures.Macro.Expressions;
using BoardFlow.Formats.Gerber.Reading.Macro.Syntax;

namespace BoardFlow.Formats.Gerber.Reading.Macro;

public class ExpressionBuilder {

    private readonly List<ISyntaxExpressionPart> _parts;
    private int _curIndex;
    public ExpressionBuilder(SyntaxExpression syntaxExpression) {
        _parts = syntaxExpression.Parts;
        _curIndex = 1;
    }

    public ExpressionBuilder(string text) : this(new SyntaxExpressionBuilder().Build(text)) { }

    public SyntaxOperator Current {
        get {
            if (_parts[_curIndex] is not SyntaxOperator) {
                throw new Exception("Invalid operation expression");
            }
            return (_parts[_curIndex] as SyntaxOperator)!;
        }
    }
    public SyntaxOperator? LeftOperator {
        get {
            if (_curIndex - 2 < 0)
                return null;
            if (_parts[_curIndex-2] is not SyntaxOperator) {
                throw new Exception("Invalid operation expression");
            }
            return (_parts[_curIndex - 2] as SyntaxOperator)!;
        }
    }
    public SyntaxOperator? RightOperator {
        get {
            if (_curIndex + 2 > _parts.Count - 1)
                return null;
            if (_parts[_curIndex + 2] is not SyntaxOperator) {
                throw new Exception("Invalid operation expression");
            }
            return (_parts[_curIndex + 2] as SyntaxOperator)!;
        }
    }
    public ISyntaxOperand? LeftOperand {
        get {
            if (_curIndex - 1 < 0)
                return null;
            if (_parts[_curIndex-1] is not ISyntaxOperand) {
                throw new Exception("Invalid operation expression");
            }
            return (_parts[_curIndex - 1] as ISyntaxOperand)!;
        }
    }
    public ISyntaxOperand? RightOperand {
        get {
            if (_curIndex + 1 > _parts.Count - 1)
                return null;
            if (_parts[_curIndex + 1] is not ISyntaxOperand) {
                throw new Exception("Invalid operation expression");
            }
            return (_parts[_curIndex + 1] as ISyntaxOperand)!;
        }
    }
    public bool Next() {
        if (_curIndex + 2 > _parts.Count - 1) 
            return false;
        _curIndex+=2;
        return true;
    }
    private static IExpression BuildOperand(ISyntaxOperand operand) {
        return operand switch {
            SyntaxGroup group => new ExpressionBuilder(group.Expression ?? throw new ApplicationException("Invalid Group operand")).Build(),
            SyntaxParameter parameter => new ParameterExpression(parameter.Name),
            SyntaxValue value => new ValueExpression(value.Value),
            _ => throw new Exception("Unknown SyntaxOperand type")
        };
    }

    private OperationExpression BuildExpression() {
        var stack = new List<OperationExpression>();
        do {
            var curExpr = new OperationExpression(Current.OperationType);
            if (LeftOperator == null || Current.Priority > LeftOperator.Priority) {
                curExpr.LeftExpression = BuildOperand(LeftOperand!);
            } else {
                curExpr.LeftExpression = stack.Pop();
            }

            if (RightOperator == null || Current.Priority >= RightOperator.Priority) {
                curExpr.RightExpression = BuildOperand(RightOperand!);
                //проверка на приоритет
                if (stack.Count != 0 &&  (RightOperator == null || RightOperator!.Priority <= stack.Last().OperationType.GetPriority())) {
                    var prev = stack.Pop();
                    prev.RightExpression = curExpr;
                    stack.Push(prev);
                } else {
                    stack.Push(curExpr);
                }
                
            } else {
                if (stack.Count != 0) {
                    var item = stack.Pop();
                    if (item.RightExpression == null) {
                        item.RightExpression = curExpr;
                        stack.Push(item);
                    }
                } else
                    stack.Push(curExpr);
            }
        } while (Next());

        return stack.Pop();
    }

    public IExpression Build() {
        return _parts.Count == 1 ? BuildOperand((_parts[0] as ISyntaxOperand)!) : BuildExpression();
    }
}


public static class ListExtension {
    public static T Pop<T>(this List<T> list) {
        var result = list[^1];
        list.RemoveAt(list.Count - 1);
        return result;
    }
    
    public static void Push<T>(this List<T> list, T item) {
        list.Add(item);
    }
}