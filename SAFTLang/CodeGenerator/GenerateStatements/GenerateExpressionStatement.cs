using System.Text;
using SAFTLang.AST.Statements;

namespace SAFTLang.CodeGenerator.GenerateStatements;

internal sealed partial class StatementGenerator
{
    private void GenerateExpressionStatement(StringBuilder output, ExpressionStatement expressionStatement, string indentation)
    {
        output.AppendLine(
            $"{indentation}" +
            $"{_expressionGenerator.GenerateExpression(expressionStatement.Expression)};"
        );
    }
}