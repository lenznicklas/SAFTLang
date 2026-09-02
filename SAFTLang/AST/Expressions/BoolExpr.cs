using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Expressions;

public record BoolExpr(
    bool Value,
    SourceSpan Span
    ) : Expr(Span);
