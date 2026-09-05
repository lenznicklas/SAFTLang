using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;

namespace SAFTLang.SemanticAnalyzer.AnalyzeExpressions;

internal sealed partial class ExpressionAnalyzer
{
    private LangType AnalyzeLenCall(CallExpr call)
    {
        if (call.Arguments.Count != 1)
        {
            _diagnostics.ReportError(call.Span, "'len' can only have one argument");
            return LangType.Error;
        }

        LangType type = AnalyzeExpression(call.Arguments[0]);

        if (type == LangType.Error)
        {
            return LangType.Error;
        }
        
        if (type.Kind != LangTypeKind.Array && type != LangType.String)
        {
            _diagnostics.ReportError(call.Arguments[0].Span, "'len' can only have arrays and strings as argument");
            return LangType.Error;
        }


        return LangType.Int;
    }
}