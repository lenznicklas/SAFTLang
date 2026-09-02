using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;

namespace SAFTLang.SemanticAnalyzer.AnalyzeExpressions;

internal sealed partial class ExpressionAnalyzer
{
    private LangType AnalyzeIndex(IndexExpr index)
    {
        LangType targetType = AnalyzeExpression(index.Target);

        LangType indexType = AnalyzeExpression(index.Index);

        if (targetType == LangType.Error ||
            indexType == LangType.Error)
        {
            return LangType.Error;
        }

        if (targetType.Kind != LangTypeKind.Array)
        {
            _diagnostics.ReportError(
                index.Target.Span,
                $"Cannot index value of type {targetType}"
            );

            return LangType.Error;
        }

        if (indexType != LangType.Int)
        {
            _diagnostics.ReportError(
                index.Index.Span,
                $"Index must be int, but got {indexType}"
            );
            return LangType.Error;
        }

        if (targetType.ElementType is null)
        {
            throw new InvalidOperationException(
                "Internal compiler error: array has no element type"
            );
        }
        
        return targetType.ElementType;
    }

}