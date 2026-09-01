using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record IndexExpr(
    Expr Target,
    Expr Index,
    SourceSpan Span
    ) : Expr(Span);