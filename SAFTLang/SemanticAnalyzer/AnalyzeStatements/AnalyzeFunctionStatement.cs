using SAFTLang.AST.Statements;
using SAFTLang.AST.Types;

namespace SAFTLang.SemanticAnalyzer.AnalyzeStatements;

internal sealed partial class StatementAnalyzer
{
    private void AnalyzeFunctionStatement(FunctionStatement functionStatement)
    {
        FunctionStatement? previousFunction = _state.CurrentFunction;
        
        _state.CurrentFunction = functionStatement;
        
        _state.BeginScope();

        try
        {
            foreach (Parameter parameter in functionStatement.Parameters)
            {
                _state.DeclareVariable(
                    parameter.Name,
                    parameter.Type,
                    isConst: false,
                    parameter.Span
                );
            }

            foreach (Statement statement in functionStatement.Body.Statements)
            {
                AnalyzeStatement(statement);
            }

            if (functionStatement.ReturnType != LangType.Void &&
                !_controlFlow.AlwaysReturns(functionStatement.Body))
            {
                _diagnostics.ReportError(
                    functionStatement.Span,
                    $"'{functionStatement.Name}' has to return a value of type {functionStatement.ReturnType}"
                );
            }
        }
        finally
        {
            _state.EndScope();

            _state.CurrentFunction = previousFunction;
        }
    }

}