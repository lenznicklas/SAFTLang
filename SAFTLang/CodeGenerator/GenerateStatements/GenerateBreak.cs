using System.Text;

namespace SAFTLang.CodeGenerator.GenerateStatements;

internal sealed partial class StatementGenerator
{
    private void GenerateBreak(StringBuilder output, string indentation)
    {
        output.AppendLine($"{indentation}break;");
    }
}