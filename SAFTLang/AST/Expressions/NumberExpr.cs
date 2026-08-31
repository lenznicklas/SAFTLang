using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record IntegerExpr(
    string Value,
    SourceSpan Span
    ) : Expr(Span);
