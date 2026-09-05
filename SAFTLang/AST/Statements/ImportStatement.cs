using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Statements;

public record ImportStatement(
    IReadOnlyList<string> Path,
    IReadOnlyList<string> Members,
    string? Alias,
    SourceSpan Span
    ) : Statement(Span);