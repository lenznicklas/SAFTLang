using SAFTLang.Lexer.Text;

namespace SAFTLang.Diagnostics;

public class DiagnosticBag
{
    private readonly List<Diagnostic> _diagnostics = new();
    
    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;
    
    public bool HasErrors =>
    _diagnostics.Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);

    public void ReportError(
        SourceSpan span,
        string message)
    {
        _diagnostics.Add(
            new Diagnostic(DiagnosticSeverity.Error, span, message)
            );
    }

    public void ReportWarning(
        SourceSpan span,
        string message)
    {
        _diagnostics.Add(
            new Diagnostic(DiagnosticSeverity.Warning, span, message)
        );
    }
    
    public void AddRange(IEnumerable<Diagnostic> diagnostics)
    {
        _diagnostics.AddRange(diagnostics);
    }
}