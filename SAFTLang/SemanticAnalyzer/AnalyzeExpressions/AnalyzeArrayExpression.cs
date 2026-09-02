using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;

namespace SAFTLang.SemanticAnalyzer.AnalyzeExpressions;

internal sealed partial class ExpressionAnalyzer
{
    private LangType AnalyzeArray(ArrayExpr array, LangType? expected)
    {
        LangType? expectedElementType = null;

        if (expected is not null &&
            expected.Kind == LangTypeKind.Array)
        {
            expectedElementType = expected.ElementType;
        }

        if (array.Elements.Count == 0)
        {
            if (expected is null ||
                expected.Kind != LangTypeKind.Array)
            {
                _diagnostics.ReportError(
                    array.Span,
                    $"Cannot infer type of empty array");
                return LangType.Error;
            }

            return expected;
        }

        LangType firstType = AnalyzeExpression(array.Elements[0], expectedElementType);
        if (firstType == LangType.Void)
        {
            _diagnostics.ReportError(
                array.Span,
                $"Array elements cannot be void");

            return LangType.Error;
        }

        if (firstType == LangType.Error)
        {
            return LangType.Error;
        }

        if (expectedElementType is not null &&
            firstType != LangType.Error &&
            firstType != expectedElementType)
        {
            _diagnostics.ReportError(
                array.Elements[0].Span,
                $"Array elements must be of type {expectedElementType} but got {firstType}"
            );
        }

        LangType elementType = expectedElementType ?? firstType;

        foreach (Expr expr in array.Elements.Skip(1))
        {
            LangType actualType = AnalyzeExpression(expr,  elementType);

            if (actualType == LangType.Error)
            {
                continue;
            }

            if (actualType != elementType)
            {
                _diagnostics.ReportError(
                    expr.Span,
                    $"Array element must be {elementType} but got {actualType}"
                );
            }
        }

        return LangType.ArrayOf(elementType);
    }

}