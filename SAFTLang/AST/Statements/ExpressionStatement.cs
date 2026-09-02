using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Statements;

public record ExpressionStatement(
    Expr Expression,
    SourceSpan Span
    ) : Statement(Span);
    