using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.SemanticAnalyzer.AnalyzeExpressions;

internal sealed partial class ExpressionAnalyzer
{
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

}