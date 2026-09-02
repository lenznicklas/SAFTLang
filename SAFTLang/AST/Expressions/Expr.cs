using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Expressions;

public abstract record Expr(
    SourceSpan Span
    )  : SyntaxNode(Span);