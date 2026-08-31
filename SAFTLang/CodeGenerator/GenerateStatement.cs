using System.Text;
using SAFTLang.AST;

namespace SAFTLang.CodeGenerator;

public partial class CodeGenerator
{
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

                GenerateStatement(
                    output, ifStatement.thenBody, indent
                );

                if (ifStatement.elseBody is not null)
                {
                    output.AppendLine($"{indentation}else");

                    GenerateStatement(
                        output, ifStatement.elseBody, indent
                    );
                }
                
                break;
            case ExpressionStatement expressionStatement:
                output.AppendLine(
                    $"{indentation}" +
                    $"{GenerateExpression(expressionStatement.Expression)};"
                );
                break;
            case BlockStatement blockStatement:
                GenerateBlockStatement(
                    output,
                    blockStatement,
                    indent
                );
                break;
            case AssignmentStatement assignmentStatement:
                output.AppendLine(
                    $"{indentation}" +
                    $"{GenerateIdentifier(assignmentStatement.Name)} = " +
                    $"{GenerateExpression(assignmentStatement.Value)};"
                );
                break;
            case ReturnStatement returnStatement:
                if (returnStatement.Value is null)
                {
                    output.AppendLine($"{indentation}return;");
                }
                else
                {
                    output.AppendLine($"{indentation}return {returnStatement.Value};");
                }
                break;
            default:
                throw new Exception($"Unknown statement {statement.GetType().Name}");
        }
    }

    private void GenerateBlockStatement(
        StringBuilder output,
        BlockStatement block,
        int indent
    )
    {
        string indentation = new string(' ', indent * 4);

        output.AppendLine($"{indentation}{{");

        foreach (Statement statement in block.Statements)
        {
            GenerateStatement(
                output,
                statement,
                indent + 1
            );
        }
        
        output.AppendLine($"{indentation}}}");
    }

}