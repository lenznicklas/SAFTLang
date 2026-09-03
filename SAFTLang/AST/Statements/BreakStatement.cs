using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Statements;

public record BreakStatement(
    SourceSpan Span
    ) : Statement(Span);