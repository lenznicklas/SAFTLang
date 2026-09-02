using SAFTLang.AST.Types;
using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Statements;

public record FunctionStatement
(
    string Name, 
    IReadOnlyList<Parameter> Parameters,
    LangType ReturnType,
    BlockStatement Body,
    SourceSpan Span
) : Statement(Span);