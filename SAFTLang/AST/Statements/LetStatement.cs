using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record LetStatement(
    string Name,
    Expr Value,
    SourceSpan Span
) : Statement(Span);
