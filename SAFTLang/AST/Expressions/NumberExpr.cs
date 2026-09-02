using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Expressions;

public record IntegerExpr(
    string Value,
    SourceSpan Span
    ) : Expr(Span);
