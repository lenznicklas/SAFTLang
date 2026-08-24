using SAFTLang.Lexer;
using SAFTLang.AST;

namespace SAFTLang.SemanticAnalyzer;

public partial class SemanticAnalyzer
{
    private readonly Stack<Dictionary<string, LangType>> _scopes = new();
    private readonly Dictionary<Statement, LangType> _statementTypes = new();
    public SemanticAnalyzer()
    {
        _scopes.Push(new Dictionary<string, LangType>());
    }

    private void BeginScope()
    {
        _scopes.Push(new Dictionary<string, LangType>());
    }

    private void EndScope()
    {
        _scopes.Pop();
    }

    private void DeclareVariable(
        string name,
        LangType type)
    {
        Dictionary<string, LangType> currentScope = _scopes.Peek();

        if (currentScope.ContainsKey(name))
        {
            throw new Exception(
                $"Variable '{name}' is already defined in this scope"
            );
        }
        
        currentScope.Add(name, type);
    }

    private LangType ResolveVariable(string name)
    {
        foreach (Dictionary<string, LangType> scope in _scopes)
        {
            if (scope.TryGetValue(name, out LangType type))
            {
                return type;
            }
        }
        
        throw new Exception($"Variable '{name}' not found");
    }
    public void Analyze(List<Statement> statements)
    {
        foreach (var statement in statements)
        {
            AnalyzeStatement(statement);
        }
    }

    public LangType GetStatementType(Statement statement)
    {
        if (!_statementTypes.TryGetValue(
                statement,
                out LangType type))
        {
            throw new Exception(
                $"No type information for statement " +
                $"'{statement.GetType().Name}'"
            );
        }

        return type;
    }


}