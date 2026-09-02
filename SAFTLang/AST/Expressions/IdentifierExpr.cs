using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Expressions;

public record IdentifierExpr(
    string Name,
    SourceSpan Span
    ) : Expr(Span);
