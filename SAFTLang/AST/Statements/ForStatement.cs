using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Statements;

public record ForStatement(
    BlockStatement Block,
    SourceSpan Span
    ) : Statement(Span);