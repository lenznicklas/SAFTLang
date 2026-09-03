using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.SemanticAnalyzer.AnalyzeExpressions;

internal sealed partial class ExpressionAnalyzer
{
    private LangType AnalyzeUnary(UnaryExpr unary)
    {
        LangType operandType = AnalyzeExpression(unary.Operand);

        if (operandType == LangType.Error)
        {
            return LangType.Error;
        }

        switch (unary.Operator)
        {
            case TokenType.Minus:
                if (operandType != LangType.Int)
                {
                    _diagnostics.ReportError(
                        unary.Span,
                        "Unary operator '-' can only be used with int"
                    );
                    return LangType.Error;
                }
                return LangType.Int;
            
            case TokenType.Not:
                if (operandType != LangType.Bool)
                {
                    _diagnostics.ReportError(
                        unary.Span,
                        "Unary operator '!' can only be used with bool"
                    );
                    return LangType.Error;
                }

                return LangType.Bool;
            
            default:
                _diagnostics.ReportError(
                    unary.Span,
                    $"Unknown unary operator '{unary.Operator}'"
                );
                return LangType.Error;
        }

    }
}