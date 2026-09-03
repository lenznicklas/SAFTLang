using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Statements;

public record ForEachStatement(
    string VariableName,
    Expr Iterable,
    BlockStatement Body,
    SourceSpan Span
    ) : Statement(Span);