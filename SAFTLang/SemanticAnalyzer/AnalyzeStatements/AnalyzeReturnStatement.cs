using SAFTLang.AST.Statements;
using SAFTLang.AST.Types;

namespace SAFTLang.SemanticAnalyzer.AnalyzeStatements;

internal sealed partial class StatementAnalyzer
{
    private void AnalyzeReturnStatement(ReturnStatement statement)
    {
        if (_state.CurrentFunction is null)
        {
            _diagnostics.ReportError(
                statement.Span,
                "Return statement is only allowed inside a function"
            );
            
            return;
        }

        LangType expectedType = _state.CurrentFunction.ReturnType;

        if (expectedType == LangType.Void)
        {
            if (statement.Value is not null)
            {
                _expressionAnalyzer.AnalyzeExpression(statement.Value);
                _diagnostics.ReportError(
                    statement.Span,
                    $"Void func '{_state.CurrentFunction.Name}' cannot return a value"
                );
            }
            
            return;
        }

        if (statement.Value is null)
        {
            _diagnostics.ReportError(
                statement.Span,
                $"Function '{_state.CurrentFunction.Name}' has to return a value of type {expectedType}"
            );
            
            return;
        }

        LangType actualType = _expressionAnalyzer.AnalyzeExpression(statement.Value, expectedType);

        if (actualType == LangType.Error)
        {
            return;
        }

        if (actualType != expectedType)
        {
            _diagnostics.ReportError(
                statement.Span,
                $"Expected func '{_state.CurrentFunction.Name}' to return a value of {expectedType} not {actualType}"
            );
        }
    }

}