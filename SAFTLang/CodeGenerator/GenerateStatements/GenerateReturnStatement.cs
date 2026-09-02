using System.Text;
using SAFTLang.AST.Statements;

namespace SAFTLang.CodeGenerator.GenerateStatements;

internal sealed partial class StatementGenerator
{
    private void GenerateReturn(StringBuilder output, ReturnStatement returnStatement, string indentation)
    {
        if (returnStatement.Value is null)
        {
            output.AppendLine($"{indentation}return;");
        }
        else
        {
            output.AppendLine($"{indentation}return {_expressionGenerator.GenerateExpression(returnStatement.Value)};");
        }

    }
}