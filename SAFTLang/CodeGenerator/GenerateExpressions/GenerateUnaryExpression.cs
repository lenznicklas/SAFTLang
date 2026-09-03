
using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.CodeGenerator.GenerateExpressions;

internal sealed partial class ExpressionGenerator
{
    private string GenerateUnaryExpression(UnaryExpr expr)
    {
        string operand = GenerateExpression(expr.Operand);

        string op = expr.Operator switch
        {
            TokenType.Minus => "-",
            TokenType.Not => "!",

            _ => throw new InvalidOperationException("Unknown unary operator")
        };
        
        return "({op}{operand})";
    }
}