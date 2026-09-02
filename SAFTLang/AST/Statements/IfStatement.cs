using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Statements;

public record IfStatement(
    Expr Condition, 
    BlockStatement thenBody,
    BlockStatement? elseBody,
    SourceSpan Span
    ) : Statement(Span);
