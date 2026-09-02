using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.CodeGenerator.GenerateExpressions;

internal sealed partial class ExpressionGenerator
{
    private string GenerateBinaryExpression(BinaryExpr binary)
    {
        
        string left = GenerateExpression(binary.Left);
        string right = GenerateExpression(binary.Right);

        string op = binary.Operator switch
        {
            TokenType.Plus => "+",
            TokenType.Minus => "-",
            TokenType.Star => "*",
            TokenType.Slash => "/",
                
            TokenType.EqualEqual => "==",
            TokenType.NotEqual => "!=",
            TokenType.Less => "<",
            TokenType.LessEqual => "<=",
            TokenType.Greater => ">",
            TokenType.GreaterEqual => ">=",

            _ => throw new Exception($"Unknown operator: {binary.Operator}")
        };
        return $"({left} {op} {right})";
        
    }

}