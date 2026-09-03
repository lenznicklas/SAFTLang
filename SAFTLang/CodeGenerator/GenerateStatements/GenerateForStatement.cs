using System.Text;
using SAFTLang.AST.Statements;

namespace SAFTLang.CodeGenerator.GenerateStatements;

internal sealed partial class StatementGenerator
{
    private void GenerateFor(StringBuilder output, ForStatement forStatement, string indentation, int indent)
    {
        output.AppendLine($"{indentation}while (true)");
        GenerateBlock(output, forStatement.Block, indent);
    }
}