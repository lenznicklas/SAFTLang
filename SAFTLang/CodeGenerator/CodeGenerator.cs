using System.Text;
using SAFTLang.Lexer;
using SAFTLang.AST;

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

        output.AppendLine("#include <stdio.h>");
        output.AppendLine("#include <stdbool.h>");
        output.AppendLine();
        output.AppendLine("int main(void)");
        output.AppendLine("{");

        foreach (var statement in statements)
        {
            GenerateStatement(output, statement, 1);
        }
        
        output.AppendLine();
        output.AppendLine("    return 0;");
        output.AppendLine("}");
        return output.ToString();
    }

    private void GenerateStatement(
        StringBuilder output,
        Statement statement,
        int indent)
    {
        string indentation = new string(' ', indent * 4);

        switch (statement)
        {
            case LetStatement let:
                output.AppendLine(
                    $"{indentation}" +
                    $"{GenerateType(_analyzer.GetStatementType(let))} " +
                    $"{GenerateIdentifier(let.Name)} = {GenerateExpression(let.Value)};"
                );
                break;
            case ConstStatement constStatement:
                output.AppendLine(
                    $"{indentation}const " +
                    $"{GenerateType(_analyzer.GetStatementType(constStatement))} " +
                    $"{GenerateIdentifier(constStatement.Name)} = {GenerateExpression(constStatement.Value)};"
                );
                break;
            case IfStatement ifStatement:
                output.AppendLine(
                    $"{indentation}if " +
                    $"({GenerateExpression(ifStatement.Condition)})"
                );

                output.AppendLine($"{indentation}{{");

                foreach (Statement bodyStatement in ifStatement.Body)
                {
                    GenerateStatement(output, bodyStatement, indent + 1);
                }
                output.AppendLine($"{indentation}}}");
                break;
            
            default:
                throw new Exception($"Unknown statement {statement.GetType().Name}");
        }
    }

    private string GenerateExpression(Expr expr)
    {
        return expr switch
        {
            NumberExpr num =>
                num.Value,

            IdentifierExpr ident =>
                GenerateIdentifier(ident.Name),

            BinaryExpr binary =>
                GenerateBinaryExpression(binary),
            
            BoolExpr boolean =>
                boolean.Value ? "true" : "false",
            
            StringExpr str =>
                $"\"{EscapeCString(str.Value)}\"",

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

    private string GenerateType(LangType type)
    {
        return type switch
        {
            LangType.Int => "int",
            LangType.Bool => "bool",
            LangType.String => "const char*",
            _ => throw new Exception($"Unknown type {type.GetType().Name}")
        };
    }

    private string EscapeCString(string str)
    {
        return str
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }

    private string GenerateIdentifier(string name)
    {
        return $"saft_{name}";
    }
}