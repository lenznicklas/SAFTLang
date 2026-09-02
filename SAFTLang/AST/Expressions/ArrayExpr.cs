using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Expressions;

public record ArrayExpr(
    IReadOnlyList<Expr> Elements,
    SourceSpan Span
    ) : Expr(Span);