using SAFTLang.AST.Expressions;
using SAFTLang.AST.Statements;
using SAFTLang.AST.Types;
using SAFTLang.Diagnostics;
using SAFTLang.SemanticAnalyzer.Symbols;
using SAFTLang.Lexer.Text;

namespace SAFTLang.SemanticAnalyzer;

internal sealed class SemanticAnalyzerState
{
    private readonly Stack<Dictionary<string, VariableSymbol>> _scopes = new();
    private readonly Dictionary<string, FunctionSymbol> _functions = new();

    private readonly Dictionary<Statement, LangType> _statementTypes = new();
    private readonly Dictionary<Expr, LangType> _expressionTypes = new();
    
    private readonly DiagnosticBag _diagnostics;
    
    public FunctionStatement? CurrentFunction { get; set; }

    public SemanticAnalyzerState(DiagnosticBag diagnostics)
    {
        _diagnostics = diagnostics;
        
        _scopes.Push(new Dictionary<string, VariableSymbol>());
    }

    public void BeginScope()
    {
        _scopes.Push(new Dictionary<string, VariableSymbol>());
    }

    public void EndScope()
    {
        if (_scopes.Count == 1)
        {
            throw new InvalidOperationException("Cannot close global scope");
        }

        _scopes.Pop();
    }
    
    public VariableSymbol? DeclareVariable(
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

    public VariableSymbol? ResolveVariable(string name, SourceSpan span)
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
    
    public void DeclareFunction(FunctionStatement function)
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

    public FunctionSymbol? ResolveFunction(string name, SourceSpan span)
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

    public bool TryGetFunction(string name, out FunctionSymbol? function)
    {
        return _functions.TryGetValue(name, out function);
    }

    public void SetExpressionType(Expr expr, LangType type)
    {
        _expressionTypes[expr] = type;
    }

    public LangType GetExpressionType(Expr expr)
    {
        if (_expressionTypes.TryGetValue(expr, out LangType? type))
        {
            return type;
        }

        throw new InvalidOperationException($"No type information for {expr.GetType().Name}");
    }

    public void SetStatementType(Statement statement, LangType type)
    {
        _statementTypes[statement] = type;
    }

    public LangType GetStatementType(Statement statement)
    {
        if (_statementTypes.TryGetValue(statement,out LangType? type))
        {
            return type;
        }
        
        throw new InvalidOperationException($"No type information for {statement.GetType().Name}");
    }

}