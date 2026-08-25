using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record IdentifierExpr(
    string Name,
    SourceSpan Span
    ) : Expr(Span);
