using SAFTLang.Lexer;
using SAFTLang.Parser;

namespace SAFTLang.SemanticAnalyzer;

public class SemanticAnalyzer
{
    private readonly Dictionary<string, LangType> _variables = new();
    
    public LangType GetVariableType(string name)
    {
        return _variables[name];
    }

    public void Analyze(List<Statement> statements)
    {
        foreach (var statement in statements)
        {
            AnalyzeStatement(statement);
        }
    }

    private void AnalyzeStatement(Statement statement)
    {
        switch (statement)
        {
            case LetStatement let:
                AnalyzeLetStatement(let);
                break;
            default:
                throw new Exception($"Unknown statement {statement.GetType().Name}");
        }
    }

    private void AnalyzeLetStatement(LetStatement statement)
    {
        if (_variables.ContainsKey(statement.Name))
        {
            throw new Exception($"Variable {statement.Name} already defined");
        }

        LangType inferredType = AnalyzeExpression(statement.Value);
        
        _variables.Add(statement.Name, inferredType);
    }

    private LangType AnalyzeExpression(Expr expr)
    {
        return expr switch
        {
            NumberExpr =>
                LangType.Int,
            BoolExpr =>
                LangType.Bool,
            IdentifierExpr ident =>
                AnalyzeIdentifier(ident),
            BinaryExpr binary =>
                AnalyzeBinary(binary),
            _ => throw new Exception($"Unknown expression {expr.GetType().Name}")
        };
    }

    private LangType AnalyzeIdentifier(IdentifierExpr ident)
    {
        if (!_variables.TryGetValue(ident.Name, out LangType type))
        {
            throw new Exception($"Unknown variable '{ident.Name}'");
        }

        return type;
    }

    private LangType AnalyzeBinary(BinaryExpr binary)
    {
        LangType leftType = AnalyzeExpression(binary.Left);
        LangType rightType = AnalyzeExpression(binary.Right);

        switch (binary.Operator)
        {
            case TokenType.Plus:
            case TokenType.Minus:
            case TokenType.Star:
            case TokenType.Slash:
                if (leftType != LangType.Int || rightType != LangType.Int)
                {
                    throw new Exception(
                        $"Operator '{binary.Operator}' " +
                        $"requires Int operands, but got " +
                        $"{leftType} and {rightType}"
                    );
                }
                return LangType.Int;
            default:
                throw new Exception($"Unknown operator {binary.Operator}");
        }
    }
}