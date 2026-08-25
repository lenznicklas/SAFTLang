using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public abstract record Statement(
    SourceSpan Span
    );
    