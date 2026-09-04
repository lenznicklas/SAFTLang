using System.Text;
using SAFTLang.AST.Statements;
using SAFTLang.AST.Types;
using SAFTLang.CodeGenerator.Utils;

namespace SAFTLang.CodeGenerator.GenerateStatements;

internal sealed partial class StatementGenerator
{
        private void GenerateForEach(
        StringBuilder output,
        ForEachStatement forEachStatement,
        string indentation,
        int indent)
    {
        LangType iterableType =
            _analyzer.GetExpressionType(
                forEachStatement.Iterable
            );

        if (iterableType.Kind != LangTypeKind.Array ||
            iterableType.ElementType is null)
        {
            throw new InvalidOperationException(
                "Internal compiler error: " +
                "foreach iterable is not an array"
            );
        }

        LangType elementType =
            iterableType.ElementType;

        string cElementType =
            _typeGenerator.GenerateType(
                elementType
            );

        string iterable =
            _expressionGenerator.GenerateExpression(
                forEachStatement.Iterable
            );

        string variableName =
            CCodeUtils.GenerateIdentifier(
                forEachStatement.VariableName
            );

        string innerIndentation =
            new string(' ', (indent + 1) * 4);

        string bodyIndentation =
            new string(' ', (indent + 2) * 4);

        output.AppendLine(
            $"{indentation}{{"
        );

        output.AppendLine(
            $"{innerIndentation}" +
            $"saft_array saftc_iterable = {iterable};"
        );

        output.AppendLine();

        output.AppendLine(
            $"{innerIndentation}" +
            "for (int saftc_index = 0; " +
            "saftc_index < (int)saft_array_len(saftc_iterable); " +
            "saftc_index++)"
        );

        output.AppendLine(
            $"{innerIndentation}{{"
        );

        output.AppendLine(
            $"{bodyIndentation}" +
            $"{cElementType} {variableName} = " +
            $"*(({cElementType}*)" +
            $"saft_array_at(" +
            $"saftc_iterable, saftc_index));"
        );

        output.AppendLine();

        foreach (Statement statement
                 in forEachStatement.Body.Statements)
        {
            GenerateStatement(
                output,
                statement,
                indent + 2
            );
        }

        output.AppendLine(
            $"{innerIndentation}}}"
        );

        output.AppendLine(
            $"{indentation}}}"
        );
    }
}