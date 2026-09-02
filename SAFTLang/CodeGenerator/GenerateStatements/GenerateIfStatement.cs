using System.Text;
using SAFTLang.AST.Statements;

namespace SAFTLang.CodeGenerator.GenerateStatements;

internal sealed partial class StatementGenerator
{
    public void GenerateIf(StringBuilder output, IfStatement ifStatement, string indentation, int indent)
    {
        output.AppendLine(
            $"{indentation}if " +
            $"({_expressionGenerator.GenerateExpression(ifStatement.Condition)})"
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

    }
}