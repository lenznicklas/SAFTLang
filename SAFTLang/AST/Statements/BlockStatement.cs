using SAFTLang.Lexer.Text;

namespace SAFTLang.AST;

public record BlockStatement(
    List<Statement> Statements,
    SourceSpan Span
    ) : Statement(Span);
