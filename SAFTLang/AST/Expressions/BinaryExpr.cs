using SAFTLang.Lexer;

namespace SAFTLang.AST;

public record BinaryExpr(
    Expr Left, 
    TokenType Operator, 
    Expr Right
    ) : Expr;
