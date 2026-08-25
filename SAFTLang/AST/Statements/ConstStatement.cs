using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record ConstStatement(
    string Name, 
    Expr Value,
    SourceSpan  Span
    ) : Statement(Span);
