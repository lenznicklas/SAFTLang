using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Statements;

public record ReturnStatement(
    Expr? Value,
    SourceSpan Span
    ) : Statement(Span);