using SAFTLang.AST;

namespace SAFTLang.SemanticAnalyzer;

public partial class SemanticAnalyzer
{
    private LangType ReportUnknownExpression(Expr expr)
    {
        _diagnostics.ReportError(
            expr.Span,
            $"Unknown expression: {expr.GetType().Name}"
        );
        return LangType.Error;
    }
}