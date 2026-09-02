using SAFTLang.AST.Statements;
using SAFTLang.AST.Types;

namespace SAFTLang.SemanticAnalyzer.AnalyzeStatements;

internal sealed partial class StatementAnalyzer
{
    private void AnalyzeIfStatement(IfStatement statement)
    {
        LangType conditionType = _expressionAnalyzer.AnalyzeExpression(statement.Condition);

        if (conditionType != LangType.Bool &&
            conditionType != LangType.Error)
        {
            _diagnostics.ReportError(
                statement.Condition.Span,
                $"If condition must be Bool, got {conditionType}"
            );
        }
        
        AnalyzeBlockStatement(statement.thenBody);

        if (statement.elseBody is not null)
        {
            AnalyzeBlockStatement(statement.elseBody);
        }
    }

}