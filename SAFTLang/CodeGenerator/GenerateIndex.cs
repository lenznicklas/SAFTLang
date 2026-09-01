using SAFTLang.AST;

namespace SAFTLang.CodeGenerator;

public partial class CodeGenerator
{
    private string GenerateIndexExpression(IndexExpr index)
    {
        LangType elementType = _analyzer.GetExpressionType(index);

        string cElementType = GenerateType(elementType);

        string target = GenerateExpression(index.Target);

        string indexValue = GenerateExpression(index.Index);

        return
            $"*(({cElementType}*)" +
            $"saft_array_at(" +
            $"{target}" +
            $"{indexValue}" +
            $"sizeof({cElementType})))";
    }
}