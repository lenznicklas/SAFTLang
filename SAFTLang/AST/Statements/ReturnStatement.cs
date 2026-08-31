using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record ReturnStatement(
    Expr? Value,
    SourceSpan Span
    ) : Statement(Span);