using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record FunctionStatement
(
    string Name, 
    IReadOnlyList<Parameter> Parameters,
    LangType ReturnType,
    BlockStatement Body,
    SourceSpan Span
) : Statement(Span);