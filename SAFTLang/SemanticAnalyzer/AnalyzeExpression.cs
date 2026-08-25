using SAFTLang.AST;
using SAFTLang.Lexer;
using SAFTLang.Lexer.Text;
using SAFTLang.SemanticAnalyzer.Symbols;

namespace SAFTLang.SemanticAnalyzer;

public partial class SemanticAnalyzer
{
    private LangType AnalyzeExpression(Expr expr)
    {
        return expr switch
        {
            NumberExpr =>
                LangType.Int,
            BoolExpr =>
                LangType.Bool,
            StringExpr =>
                LangType.String,
            IdentifierExpr ident =>
                AnalyzeIdentifier(ident),
            BinaryExpr binary =>
                AnalyzeBinary(binary),
            ErrorExpr =>
                LangType.Error,
            _ => ReportUnknownExpression(expr)
        
        };
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
    

}