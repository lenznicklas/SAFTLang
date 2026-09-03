using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;

namespace SAFTLang.CodeGenerator.GenerateExpressions;

internal sealed partial class ExpressionGenerator
{
    private string GenerateArrayExpression(ArrayExpr array)
    {
        LangType arrayType = _analyzer.GetExpressionType(array);

        if (arrayType.Kind != LangTypeKind.Array ||
            arrayType.ElementType is null)
        {
            throw new InvalidOperationException(
                "Internal compiler error: ArrayExpr does not have an array type"
            );
        }

        LangType elementType = arrayType.ElementType;

        string cElementType = _typeGenerator.GenerateType(elementType);

        string equalityFunction = _typeGenerator.GenerateEqualityFunction(elementType);
        
        if (array.Elements.Count == 0)
        {
            return "saft_array_copy( " +
                   "NULL, " +
                   $"sizeof({cElementType})," +
                   $"0, " +
                   $"{equalityFunction})";
        }

        string elements = string.Join(", ", array.Elements.Select(GenerateExpression));

        return $"saft_array_copy(" +
               $"({cElementType}[]){{{elements}}}, " +
               $"sizeof({cElementType}), " +
               $"{array.Elements.Count}," +
               $"{equalityFunction})";

    }

}