using SAFTLang.Lexer.Text;

namespace SAFTLang.Diagnostics;

public record Diagnostic(
    SourceSpan Span,
    string Message
);