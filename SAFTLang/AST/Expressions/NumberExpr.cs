using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record NumberExpr(
    string Value,
    SourceSpan Span
    ) : Expr(Span);
