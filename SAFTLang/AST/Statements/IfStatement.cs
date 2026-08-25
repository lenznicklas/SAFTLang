using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record IfStatement(
    Expr Condition, 
    BlockStatement thenBody,
    BlockStatement? elseBody,
    SourceSpan Span
    ) : Statement(Span);
