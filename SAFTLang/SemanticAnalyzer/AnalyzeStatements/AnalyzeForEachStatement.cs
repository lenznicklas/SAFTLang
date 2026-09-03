using SAFTLang.AST.Statements;
using SAFTLang.AST.Types;

namespace SAFTLang.SemanticAnalyzer.AnalyzeStatements;

internal sealed partial class StatementAnalyzer
{
    private void AnalyzeForEachStatement(ForEachStatement stmt)
    {
        LangType iter = _expressionAnalyzer.AnalyzeExpression(stmt.Iterable);

        if (iter.Kind != LangTypeKind.Array || iter.ElementType is null)
        {
            _diagnostics.ReportError(stmt.Iterable.Span, $"Cannot iterate over {iter}");
            return;
        }
        
        _state.BeginScope();

        try
        {
            _state.DeclareVariable(stmt.VariableName, iter.ElementType, false, stmt.Span);

            foreach (Statement bodyStatement in stmt.Body.Statements)
            {
                AnalyzeStatement(bodyStatement);
            }
        }
        finally
        {
            _state.EndScope();
        }
        
    }
}