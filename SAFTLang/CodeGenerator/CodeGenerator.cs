using System.Text;
using SAFTLang.Lexer;
using SAFTLang.Parser;

namespace SAFTLang.CodeGenerator;

public class CodeGenerator
{
    private readonly SemanticAnalyzer.SemanticAnalyzer _analyzer;

    public CodeGenerator(SemanticAnalyzer.SemanticAnalyzer analyzer)
    {
        _analyzer = analyzer;
    }
    
    public string Generate(List<Statement> statements)
    {
        var output = new StringBuilder();

        output.AppendLine("#include <stdio.h");
        output.AppendLine();
        output.AppendLine("int main(void)");
        output.AppendLine("{");

        foreach (var statement in statements)
        {
            output.AppendLine($"    {GenerateStatement(statement)}");
        }
        
        output.AppendLine();
        output.AppendLine("    return 0;");
        output.AppendLine("}");
        return output.ToString();
    }

    private string GenerateStatement(Statement statement)
    {
        return statement switch
        {
            LetStatement let =>
                $"{GenerateType(_analyzer.GetVariableType(let.Name))} " +
                $"{let.Name} = {GenerateExpression(let.Value)};",

            _ => throw new Exception($"Unknown statement {statement.GetType().Name}")
        };
    }

    private string GenerateExpression(Expr expr)
    {
        return expr switch
        {
            NumberExpr num =>
                num.Value,

            IdentifierExpr ident =>
                ident.Name,

            BinaryExpr binary =>
                GenerateBinaryExpression(binary),
            
            BoolExpr boolean =>
                boolean.Value ? "1" : "0",

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

                _ => throw new Exception($"Unknown operator: {binary.Operator}")
            };
            return $"{left} {op} {right}";
        
    }

    private string GenerateType(LangType type)
    {
        return type switch
        {
            LangType.Int => "int",
            LangType.Bool => "bool",
            _ => throw new Exception($"Unknown type {type.GetType().Name}")
        };
    }
}