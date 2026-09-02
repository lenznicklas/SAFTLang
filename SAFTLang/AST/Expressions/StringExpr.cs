using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Expressions;

public record StringExpr(
    string Value,
    SourceSpan Span
    ) : Expr(Span);
