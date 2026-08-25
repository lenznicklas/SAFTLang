using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record IfStatement(
    Expr Condition, 
    BlockStatement Body,
    SourceSpan Span
    ) : Statement(Span);
