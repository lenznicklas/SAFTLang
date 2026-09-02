using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Statements;

public record BlockStatement(
    List<Statement> Statements,
    SourceSpan Span
    ) : Statement(Span);
