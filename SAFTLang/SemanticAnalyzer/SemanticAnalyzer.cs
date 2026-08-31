using SAFTLang.Lexer;
using SAFTLang.AST;
using SAFTLang.Diagnostics;
using SAFTLang.Lexer.Text;
using SAFTLang.SemanticAnalyzer.Symbols;

namespace SAFTLang.SemanticAnalyzer;

public partial class SemanticAnalyzer
{
    private readonly Stack<Dictionary<string, VariableSymbol>> _scopes = new();
    private readonly Dictionary<string, FunctionSymbol> _functions = new();

    private FunctionStatement? _currentFunction;
    
    private readonly Dictionary<Statement, LangType> _statementTypes = new();
    private readonly DiagnosticBag _diagnostics = new();

    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics.Diagnostics;
    
    public SemanticAnalyzer()
    {
        _scopes.Push(new Dictionary<string, VariableSymbol>());
    }

    private void BeginScope()
    {
        _scopes.Push(new Dictionary<string, VariableSymbol>());
    }

    private void EndScope()
    {
        if (_scopes.Count == 1)
        {
            throw new Exception("Cannot close global scope");
        }
        _scopes.Pop();
    }

    private VariableSymbol? DeclareVariable(
        string name,
        LangType type,
        bool isConst,
        SourceSpan span)
    {
        Dictionary<string, VariableSymbol> currentScope = _scopes.Peek();

        if (currentScope.ContainsKey(name))
        {
            _diagnostics.ReportError(
                span,
                $"Variable '{name}' is already defined in this scope"
            );
            return null;
        }
        
        var symbol = new VariableSymbol(name, type, isConst);
        
        currentScope.Add(name, symbol);
        return symbol;
    }

    private VariableSymbol? ResolveVariable(string name, SourceSpan span)
    {
        foreach (Dictionary<string, VariableSymbol> scope in _scopes)
        {
            if (scope.TryGetValue(name, out VariableSymbol? symbol))
            {
                return symbol;
            }
        }

        _diagnostics.ReportError(
            span,
            $"Unknown variable '{name}'"
        );

        return null;
    }
    public void Analyze(List<Statement> statements)
    {
        foreach (Statement statement in statements)
        {
            if (statement is FunctionStatement function)
            {
                DeclareFunction(function);
            }
        }

        foreach (Statement statement in statements)
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
            throw new InvalidOperationException(
                $"Internal compiler error: no type information " +
                $"for {statement.GetType().Name}"
            );
        }

        return type;
    }

    private void DeclareFunction(FunctionStatement function)
    {
        if (_functions.ContainsKey(function.Name))
        {
            _diagnostics.ReportError(function.Span,
                $"Function '{function.Name}' is already defined"
            );
            
            return;
        }

        var symbol = new FunctionSymbol(
            function.Name,
            function.Parameters
                .Select(parameter => parameter.Type)
                .ToList(),
            function.ReturnType
        );
        
        _functions.Add(function.Name, symbol);
    }

    private FunctionSymbol? ResolveFunction(string name, SourceSpan span)
    {
        if (_functions.TryGetValue(
                name,
                out FunctionSymbol? function))
        {
            return function;
        }

        _diagnostics.ReportError(
            span,
            $"Unknown function '{name}'"
        );

        return null;
    }


}