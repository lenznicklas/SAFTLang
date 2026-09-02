using System.Text;
using SAFTLang.AST.Statements;

namespace SAFTLang.CodeGenerator.GenerateStatements;

internal sealed partial class StatementGenerator
{
    private void GenerateAssignment(StringBuilder output, AssignmentStatement assignmentStatement, string indentation)
    {
        output.AppendLine(
            $"{indentation}" +
            $"{_expressionGenerator.GenerateExpression(assignmentStatement.Target)} = " +
            $"{_expressionGenerator.GenerateExpression(assignmentStatement.Value)};"
        );

    }
}