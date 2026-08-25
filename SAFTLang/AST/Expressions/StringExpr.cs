using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record StringExpr(
    string Value,
    SourceSpan Span
    ) : Expr(Span);
