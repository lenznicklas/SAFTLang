using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Statements;

public record AssignmentStatement(
    Expr Target, 
    Expr Value,
    SourceSpan Span
    ) : Statement(Span);
