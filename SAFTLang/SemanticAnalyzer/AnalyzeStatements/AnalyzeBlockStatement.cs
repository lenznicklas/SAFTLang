using SAFTLang.AST.Statements;

namespace SAFTLang.SemanticAnalyzer.AnalyzeStatements;

internal sealed partial class StatementAnalyzer
{
    private void AnalyzeBlockStatement(BlockStatement block)
    {
        _state.BeginScope();

        try
        {
            foreach (Statement statement in block.Statements)
            {
                AnalyzeStatement(statement);
            }
        }
        finally
        {
            _state.EndScope();
        }
    }

}