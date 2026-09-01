using SAFTLang.AST;
using SAFTLang.Lexer;
using SAFTLang.Lexer.Text;
using SAFTLang.SemanticAnalyzer.Symbols;

namespace SAFTLang.SemanticAnalyzer;

public partial class SemanticAnalyzer
{
    private LangType AnalyzeExpression(Expr expr, LangType? expectedType = null)
    {
        LangType type = expr switch
        {
            IntegerExpr =>
                LangType.Int,
            BoolExpr =>
                LangType.Bool,
            StringExpr =>
                LangType.String,
            IdentifierExpr ident =>
                AnalyzeIdentifier(ident),
            BinaryExpr binary =>
                AnalyzeBinary(binary),
            CallExpr call =>
                AnalyzeCall(call),
            ArrayExpr array => 
                AnalyzeArrayExpression(array, expectedType),
            IndexExpr index =>
                AnalyzeIndexExpression(index),
            ErrorExpr =>
                LangType.Error,
            _ => ReportUnknownExpression(expr)
        
        };

        _expressionTypes[expr] = type;

        return type;
    }

    private LangType AnalyzeIdentifier(IdentifierExpr ident)
    {
        VariableSymbol? symbol = ResolveVariable(ident.Name, ident.Span);
        if (symbol is null)
        {
            return LangType.Error;
        }
        
        return symbol.Type;
    }

    private LangType AnalyzeBinary(BinaryExpr binary)
    {
        LangType leftType = AnalyzeExpression(binary.Left);
        LangType rightType = AnalyzeExpression(binary.Right);

        switch (binary.Operator)
        {
            case TokenType.Plus:
            case TokenType.Minus:
            case TokenType.Star:
            case TokenType.Slash:
                if (!RequireTypes(binary.Operator, leftType, rightType, LangType.Int, binary.Span))
                {
                    return LangType.Error;
                }
                return LangType.Int;

            case TokenType.Less:
            case TokenType.Greater:
            case TokenType.LessEqual:
            case TokenType.GreaterEqual:
                if (!RequireTypes(binary.Operator, leftType, rightType, LangType.Int, binary.Span))
                {
                    return LangType.Error;
                }
                return LangType.Bool;

            case TokenType.EqualEqual:
            case TokenType.NotEqual:
                if (leftType == LangType.Error ||
                    rightType == LangType.Error)
                {
                    return LangType.Error;
                }
                if (leftType != rightType)
                {
                    _diagnostics.ReportError(
                        binary.Span,
                        $"Cannot compare {leftType} with {rightType}"
                    );
                }

                if (leftType == LangType.String)
                {
                    _diagnostics.ReportError(
                        binary.Span,
                        $"Cannot compare Strings yet"
                        );
                }

                return LangType.Bool;
            default:
                _diagnostics.ReportError(
                    binary.Span,
                    $"Unknown operator {binary.Operator}"
                );
                return LangType.Error;
        }
    }
    
    private LangType AnalyzeCall(CallExpr call)
    {
        if (call.Callee is not IdentifierExpr identifier)
        {
            _diagnostics.ReportError(
                call.Callee.Span,
                "Expression is not callable"
            );

            return LangType.Error;
        }

        if (identifier.Name == "print")
        {
            if (call.Arguments.Count != 1)
            {
                _diagnostics.ReportError(
                    call.Span,
                    $"Function 'print' expects exactly one argument"
                );

                return LangType.Error;
            }

            AnalyzeExpression(call.Arguments[0]);
            
            return LangType.Void;
        }

        FunctionSymbol? function = ResolveFunction(identifier.Name, identifier.Span);

        if (function is null)
        {
            return LangType.Error;
        }
        
        if (call.Arguments.Count != function.ParameterTypes.Count)
        {
            _diagnostics.ReportError(
                call.Span,
                $"Function '{function.Name}' expects  argument"
            );

            return LangType.Error;
        }

        for (int i = 0; i < call.Arguments.Count; i++)
        {
            Expr argument = call.Arguments[i];

            LangType expectedType = function.ParameterTypes[i];
            
            LangType actualType = AnalyzeExpression(argument, expectedType);

            if (actualType != LangType.Error &&
                actualType != expectedType)
            {
                _diagnostics.ReportError(
                    argument.Span,
                    $"Argument {i + 1} of function '{function.Name}' expects {expectedType} but got {actualType}"
                );
            }
        }
        
        return function.ReturnType;
    }

    private bool RequireTypes(
            TokenType op,
            LangType left,
            LangType right,
            LangType expected,
            SourceSpan span)
        {
            if (left == LangType.Error ||
                right == LangType.Error)
            {
                return false;
            }
            if (left != expected || right != expected)
            {
                _diagnostics.ReportError(
                    span,
                    $"Operator {op} requires type {expected} but got {left} and {right}"
                    );
                return false;
            }
            return true;
        }

    private LangType AnalyzeArrayExpression(ArrayExpr array, LangType? expected)
    {
        LangType? expectedElementType = null;

        if (expected is not null &&
            expected.Kind == LangTypeKind.Array)
        {
            expectedElementType = expected.ElementType;
        }

        if (array.Elements.Count == 0)
        {
            if (expected is null ||
                expected.Kind != LangTypeKind.Array)
            {
                _diagnostics.ReportError(
                    array.Span,
                    $"Cannot infer type of empty array");
                return LangType.Error;
            }

            return expected;
        }

        LangType firstType = AnalyzeExpression(array.Elements[0], expectedElementType);
        if (firstType == LangType.Void)
        {
            _diagnostics.ReportError(
                array.Span,
                $"Array elements cannot be void");

            return LangType.Error;
        }

        if (firstType == LangType.Error)
        {
            return LangType.Error;
        }

        if (expectedElementType is not null &&
            firstType != LangType.Error &&
            firstType != expectedElementType)
        {
            _diagnostics.ReportError(
                array.Elements[0].Span,
                $"Array elements must be of type {expectedElementType} but got {firstType}"
            );
        }

        LangType elementType = expectedElementType ?? firstType;

        foreach (Expr expr in array.Elements.Skip(1))
        {
            LangType actualType = AnalyzeExpression(expr,  elementType);

            if (actualType == LangType.Error)
            {
                continue;
            }

            if (actualType != elementType)
            {
                _diagnostics.ReportError(
                    expr.Span,
                    $"Array element must be {elementType} but got {actualType}"
                );
            }
        }

        return LangType.ArrayOf(elementType);
    }

    private LangType AnalyzeIndexExpression(IndexExpr index)
    {
        LangType targetType = AnalyzeExpression(index.Target);

        LangType indexType = AnalyzeExpression(index.Index);

        if (targetType == LangType.Error ||
            indexType == LangType.Error)
        {
            return LangType.Error;
        }

        if (targetType.Kind != LangTypeKind.Array)
        {
            _diagnostics.ReportError(
                index.Target.Span,
                $"Cannot index value of type {targetType}"
            );

            return LangType.Error;
        }

        if (indexType != LangType.Int)
        {
            _diagnostics.ReportError(
                index.Target.Span,
                $"Index must be int, but got {indexType}"
            );
            return LangType.Error;
        }

        if (targetType.ElementType is null)
        {
            throw new InvalidOperationException(
                "Internal compiler error: array has no element type"
            );
        }
        
        return targetType.ElementType;
    }

}