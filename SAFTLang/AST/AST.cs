using SAFTLang.Lexer;

namespace SAFTLang.AST;

public abstract record Expr;

public record NumberExpr(string Value) : Expr;
public record IdentifierExpr(string Name) : Expr;
public record BinaryExpr(Expr Left, TokenType Operator, Expr Right) : Expr;
public record BoolExpr(bool Value) : Expr;
public record StringExpr(string Value) : Expr;

public abstract record Statement;

public record LetStatement(string Name, Expr Value) : Statement;
public record ConstStatement(string Name, Expr Value) : Statement;
public record IfStatement(Expr Condition, BlockStatement Body) : Statement;
public record BlockStatement(List<Statement> Statements) : Statement;
public record AssignmentStatement(string Name, Expr Value) : Statement;
