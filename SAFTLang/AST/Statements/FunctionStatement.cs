using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record FunctionStatement
(
    string Name, 
    IReadOnlyList<Parameter> ParameterizedThreadStart,
    LangType ReturnType,
    BlockStatement MethodBody,
    SourceSpan Span
) : Statement(Span);