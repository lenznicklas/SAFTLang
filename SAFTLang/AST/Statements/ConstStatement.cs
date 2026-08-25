namespace SAFTLang.AST;

public record ConstStatement(
    string Name, 
    Expr Value
    ) : Statement;
