using System.Text;
using SAFTLang.AST.Statements;
using SAFTLang.CodeGenerator.Utils;

namespace SAFTLang.CodeGenerator.GenerateStatements;

internal sealed partial class StatementGenerator
{
    private void GenerateLet(StringBuilder output, LetStatement let, string indentation)
    {
        output.AppendLine(
            $"{indentation}" +
            $"{_typeGenerator.GenerateType(_analyzer.GetStatementType(let))} " +
            $"{CCodeUtils.GenerateVariableIdentifier(let.Name)} = {_expressionGenerator.GenerateExpression(let.Value)};"
        );
    }
}