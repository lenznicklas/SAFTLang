using SAFTLang.Lexer.Text;

namespace SAFTLang.Diagnostics;

public record Diagnostic(
    DiagnosticSeverity Severity,
    SourceSpan Span,
    string Message
)
{
    public override string ToString()
    {
        return $"{Span}: {Severity.ToString()}: {Message}";
    }
}