using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Statements;

public record ForStatement(
    Expr? Condition,
    BlockStatement Block,
    SourceSpan Span
    ) : Statement(Span);