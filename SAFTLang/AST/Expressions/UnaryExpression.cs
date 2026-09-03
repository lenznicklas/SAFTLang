using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.AST.Expressions;

public record UnaryExpr(
    TokenType  Operator,
    Expr Operand,
    SourceSpan Span
    ) : Expr(Span);