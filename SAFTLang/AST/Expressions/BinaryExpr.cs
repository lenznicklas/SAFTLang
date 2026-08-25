using SAFTLang.Lexer;
using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record BinaryExpr(
    Expr Left, 
    TokenType Operator, 
    Expr Right,
    SourceSpan Span
    ) : Expr(Span);
