using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record ArrayExpr(
    IReadOnlyList<Expr> Elements,
    SourceSpan Span
    ) : Expr(Span);