using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;
using SAFTLang.CodeGenerator.Utils;

namespace SAFTLang.CodeGenerator.GenerateExpressions;

internal sealed partial class ExpressionGenerator
{
    private string GenerateIndexExpression(IndexExpr index)
    {
        LangType elementType = _analyzer.GetExpressionType(index);

        string cElementType = _typeGenerator.GenerateType(elementType);

        string target = GenerateExpression(index.Target);

        string indexValue = GenerateExpression(index.Index);

        return
            $"*(({cElementType}*)" +
            $"saft_array_at(" +
            $"{target}, {indexValue}, " +
            $"sizeof({cElementType})))";
    }

}