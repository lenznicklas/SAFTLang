using SAFTLang.AST.Statements;

namespace SAFTLang.SemanticAnalyzer.AnalyzeStatements;

internal sealed partial class StatementAnalyzer
{
    private void AnalyzeBreakStatement(BreakStatement breakStatement)
    {
        if (_loopDepth == 0)
        {
            _diagnostics.ReportError(breakStatement.Span, "Break can only be used inside of a loop");
        }
    }
}