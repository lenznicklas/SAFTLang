namespace SAFTLang.AST;

public record IfStatement(
    Expr Condition, 
    BlockStatement Body
    ) : Statement;
