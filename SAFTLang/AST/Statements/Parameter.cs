using SAFTLang.AST.Types;
using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Statements;

public record Parameter(
    string Name,
    LangType Type,
    SourceSpan Span
) : Statement(Span);