using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;

namespace SAFTLang.CodeGenerator.GenerateExpressions;

internal sealed partial class ExpressionGenerator
{
    private string GenerateAppendCall(CallExpr expr)
    {
        Expr arrayExpression = expr.Arguments[0];

        Expr valueExpression = expr.Arguments[1];

        LangType arrayType = _analyzer.GetExpressionType(arrayExpression);

        if (arrayType.Kind != LangTypeKind.Array ||
            arrayType.ElementType is null)
        {
            throw new InvalidOperationException("Internal compiler error_ append target is not an array");
        }

        LangType elementType = arrayType.ElementType;
        string cElementType = _typeGenerator.GenerateType(elementType);

        string array = GenerateExpression(arrayExpression);

        string value = GenerateExpression(valueExpression);

        return
            $"saft_array_append(" +
            $"{array}, " +
            $"({cElementType}[]){{{value}}}";
    }
}