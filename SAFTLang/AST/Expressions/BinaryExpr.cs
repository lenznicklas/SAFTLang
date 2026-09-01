using SAFTLang.Lexer;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.AST;

public record BinaryExpr(
    Expr Left, 
    TokenType Operator, 
    Expr Right,
    SourceSpan Span
    ) : Expr(Span);
