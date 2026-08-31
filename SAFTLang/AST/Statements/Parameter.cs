using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record Parameter(
    string Name,
    LangType Type,
    SourceSpan Span
) : Statement(Span);