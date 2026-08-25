using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record BoolExpr(
    bool Value,
    SourceSpan Span
    ) : Expr(Span);
