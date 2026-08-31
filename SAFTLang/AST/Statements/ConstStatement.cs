using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record ConstStatement(
    string Name, 
    LangType? DeclaredType,
    Expr Value,
    SourceSpan  Span
    ) : Statement(Span);
