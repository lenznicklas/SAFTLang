using SAFTLang.AST.Statements;
using SAFTLang.AST.Types;

namespace SAFTLang.SemanticAnalyzer.AnalyzeStatements;

internal sealed partial class StatementAnalyzer
{
    private void AnalyzeForStatement(ForStatement forStatement)
    {
        if (forStatement.Condition is not null)
        {
            LangType conditionType = _expressionAnalyzer.AnalyzeExpression(forStatement.Condition);

            if (conditionType != LangType.Bool ||
                conditionType != LangType.Error)
            {
                _diagnostics.ReportError(forStatement.Span, "Condition must be of type bool");
                return;
            }
        }
        
        AnalyzeBlockStatement(forStatement.Block);
    }
}