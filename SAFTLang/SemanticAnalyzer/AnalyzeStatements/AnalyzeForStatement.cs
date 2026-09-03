using SAFTLang.AST.Statements;

namespace SAFTLang.SemanticAnalyzer.AnalyzeStatements;

internal sealed partial class StatementAnalyzer
{
    private void AnalyzeForStatement(ForStatement forStatement)
    {
        AnalyzeBlockStatement(forStatement.Block);
    }
}