using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.CodeGenerator.GenerateExpressions;

internal sealed partial class ExpressionGenerator
{
    private string GenerateBinaryExpression(BinaryExpr binary)
    {
        
        string left = GenerateExpression(binary.Left);
        string right = GenerateExpression(binary.Right);

        if (binary.Operator == TokenType.EqualEqual ||
            binary.Operator == TokenType.NotEqual)
        {
            LangType operandType = _analyzer.GetExpressionType(binary.Left);
            
            return GenerateEqualityExpression(binary, left, right, operandType);
        }
        
        string op = binary.Operator switch
        {
            TokenType.Plus => "+",
            TokenType.Minus => "-",
            TokenType.Star => "*",
            TokenType.Slash => "/",
            TokenType.Modulo => "%",
                
            TokenType.Less => "<",
            TokenType.LessEqual => "<=",
            TokenType.Greater => ">",
            TokenType.GreaterEqual => ">=",
            
            TokenType.And => "&&",
            TokenType.Or => "||",

            _ => throw new InvalidOperationException($"Unknown operator: {binary.Operator}")
        };
        return $"({left} {op} {right})";
        
    }

}