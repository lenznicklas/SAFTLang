using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record CallExpr(
    Expr Callee,
    IReadOnlyList<Expr> Arguments,
    SourceSpan Span
    ) : Expr(Span);