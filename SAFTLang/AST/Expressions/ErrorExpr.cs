using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Expressions;

public record ErrorExpr(
    SourceSpan Span
    ) : Expr(Span);