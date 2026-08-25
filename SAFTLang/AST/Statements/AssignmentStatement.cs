using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record AssignmentStatement(
    string Name, 
    Expr Value,
    SourceSpan Span
    ) : Statement(Span);
