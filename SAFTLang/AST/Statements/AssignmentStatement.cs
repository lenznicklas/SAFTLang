namespace SAFTLang.AST;

public record AssignmentStatement(
    string Name, 
    Expr Value
    ) : Statement;
