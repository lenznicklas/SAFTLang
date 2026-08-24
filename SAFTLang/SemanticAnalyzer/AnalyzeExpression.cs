using SAFTLang.AST;
using SAFTLang.Lexer;

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
            _ => throw new Exception($"Unknown expression {expr.GetType().Name}")
        };
    }

    private LangType AnalyzeIdentifier(IdentifierExpr ident)
    {
        return ResolveVariable(ident.Name);
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
                RequireTypes(binary.Operator, leftType, rightType, LangType.Int);
                return LangType.Int;

            case TokenType.Less:
            case TokenType.Greater:
            case TokenType.LessEqual:
            case TokenType.GreaterEqual:
                RequireTypes(binary.Operator, leftType, rightType, LangType.Int);
                return LangType.Bool;

            case TokenType.EqualEqual:
            case TokenType.NotEqual:
                if (leftType != rightType)
                {
                    throw new Exception(
                        $"Cannot compare {leftType} with {rightType}"
                    );
                }

                return LangType.Bool;
            default:
                throw new Exception($"Unknown operator {binary.Operator}");
        }
    }

    private void RequireTypes(
            TokenType op,
            LangType left,
            LangType right,
            LangType expected)
        {
            if (left != expected || right != expected)
            {
                throw new Exception(
                    $"Operator {op} requires two {expected} operands, " +
                    $"but got {left} and {right}."
                );
            }
        }
    

}