using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public abstract record Expr(
    SourceSpan Span
    )  : SyntaxNode;