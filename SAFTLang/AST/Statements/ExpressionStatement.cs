using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record ExpressionStatement(
    Expr Expression,
    SourceSpan Span
    ) : Statement(Span);
    