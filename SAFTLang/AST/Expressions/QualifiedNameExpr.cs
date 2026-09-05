using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Expressions;

public record QualifiedNameExpr(
    IReadOnlyList<string> Parts,
    SourceSpan Span
    ) : Expr(Span);