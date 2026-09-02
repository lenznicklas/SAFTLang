using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Statements;

public abstract record Statement(
    SourceSpan Span
    ) : SyntaxNode(Span);
    