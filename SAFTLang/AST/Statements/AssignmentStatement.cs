using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record AssignmentStatement(
    Expr Target, 
    Expr Value,
    SourceSpan Span
    ) : Statement(Span);
