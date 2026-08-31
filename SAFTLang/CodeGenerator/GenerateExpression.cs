using SAFTLang.AST;
using SAFTLang.Lexer;

namespace SAFTLang.CodeGenerator;

public partial class CodeGenerator
{
    private string GenerateExpression(Expr expr)
    {
        return expr switch
        {
            IntegerExpr num =>
                num.Value,

            IdentifierExpr ident =>
                GenerateIdentifier(ident.Name),

            BinaryExpr binary =>
                GenerateBinaryExpression(binary),
            
            BoolExpr boolean =>
                boolean.Value ? "true" : "false",
            
            StringExpr str =>
                $"\"{EscapeCString(str.Value)}\"",
            
            CallExpr call =>
                GenerateCallExpression(call),

            _ => throw new Exception($"Unknown expression {expr.GetType().Name}")
        };
    }

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

    private string GenerateCallExpression(CallExpr call)
    {
        string callee;

        if (call.Callee is IdentifierExpr ident &&
            ident.Name == "print")
        {
            return GeneratePrintCall(call);
        }
        else
        {
            callee = GenerateExpression(call.Callee);
        }
        
        string arguments = string.Join(
            ", ",
            call.Arguments.Select(GenerateExpression)
        );
        
        return $"{callee}({arguments})";
    }

    private string GeneratePrintCall(CallExpr call)
    {
        Expr expr = call.Arguments[0];

        return expr switch
        {
            BoolExpr => $"printf(\"%s\\n\", {GenerateExpression(expr)} ? \"true\" : \"false\")",
            
            StringExpr => $"printf(\"%s\\n\", {GenerateExpression(expr)})",
            
            IntegerExpr => $"printf(\"%d\\n\", {GenerateExpression(expr)})",
            
            _ => $"printf(\"%d\\n\", {GenerateExpression(expr)})"
        };
    }

}