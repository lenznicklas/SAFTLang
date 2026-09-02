using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;

namespace SAFTLang.CodeGenerator;

public partial class CodeGenerator
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

        if (array.Elements.Count == 0)
        {
            return "(saft_array){ " +
                   ".data = NULL, " +
                   ".length = 0 }";
        }

        string cElementType = GenerateType(elementType);

        string elements = string.Join(", ", array.Elements.Select(GenerateExpression));

        return $"saft_array_copy(" +
               $"({cElementType}[]){{{elements}}}, " +
               $"sizeof({cElementType}), " +
               $"{array.Elements.Count})";

    }
}