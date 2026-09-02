using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.CodeGenerator.GenerateExpressions;

internal sealed partial class ExpressionGenerator
{
    private string GenerateEqualityExpression(
        BinaryExpr binary,
        string left,
        string right,
        LangType type)
    {
        bool equals = binary.Operator == TokenType.EqualEqual;

        return type.Kind switch
        {
            LangTypeKind.String =>
                equals
                    ? $"(strcmp({left}, {right}) == 0)"
                    : $"(strcmp({left}, {right}) != 0)",

            LangTypeKind.Array =>
                equals
                    ? $"saft_array_equal({left}, {right})"
                    : $"(!saft_array_equal({left}, {right}))",

            _ =>
                $"({left} " +
                $"{(equals ? "==" : "!=")} " +
                $"{right})"
        };
    }
}