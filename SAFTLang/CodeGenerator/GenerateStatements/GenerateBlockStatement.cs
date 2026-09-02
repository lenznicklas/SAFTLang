using System.Text;
using SAFTLang.AST.Statements;

namespace SAFTLang.CodeGenerator.GenerateStatements;

internal sealed partial class StatementGenerator
{
    public void GenerateBlock(StringBuilder output, BlockStatement block, int indent)
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