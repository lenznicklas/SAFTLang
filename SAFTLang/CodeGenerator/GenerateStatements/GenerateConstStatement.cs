using System.Text;
using SAFTLang.AST.Statements;
using SAFTLang.CodeGenerator.Utils;

namespace SAFTLang.CodeGenerator.GenerateStatements;

internal sealed partial class StatementGenerator
{
    private void GenerateConst(StringBuilder output, ConstStatement constStatement, string indentation)
    {
        output.AppendLine(
            $"{indentation}const " +
            $"{_typeGenerator.GenerateType(_analyzer.GetStatementType(constStatement))} " +
            $"{CCodeUtils.GenerateVariableIdentifier(constStatement.Name)} = {_expressionGenerator.GenerateExpression(constStatement.Value)};"
        );

    }
}