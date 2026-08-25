using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record ErrorExpr(
    SourceSpan Span
    ) : Expr(Span);