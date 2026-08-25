using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public abstract record SyntaxNode
{
    SourceSpan Span;
}