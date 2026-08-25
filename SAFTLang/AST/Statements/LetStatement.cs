namespace SAFTLang.AST;

public record LetStatement(
    string Name, 
    Expr Value
    ) : Statement;
