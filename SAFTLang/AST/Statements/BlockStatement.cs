namespace SAFTLang.AST;

public record BlockStatement(
    List<Statement> Statements
    ) : Statement;
