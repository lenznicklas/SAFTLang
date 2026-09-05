using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Expressions;

public record CharExpr(
    char Value,
    SourceSpan Span
    ) : Expr(Span);