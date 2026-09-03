using System.Text;
using SAFTLang.AST.Statements;

namespace SAFTLang.CodeGenerator.GenerateStatements;

internal sealed partial class StatementGenerator
{
    private void GenerateFor(StringBuilder output, ForStatement forStatement, string indentation, int indent)
    {
        string condition = forStatement.Condition is null
                ? "true"
                : _expressionGenerator.GenerateExpression(forStatement.Condition);
        
        output.AppendLine($"{indentation}while ({condition})");
        GenerateBlock(output, forStatement.Block, indent);
    }
}