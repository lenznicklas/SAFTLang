using SAFTLang.AST.Expressions;
using SAFTLang.AST.Statements;
using SAFTLang.AST.Types;
using SAFTLang.Diagnostics;
using SAFTLang.SemanticAnalyzer.Symbols;
using SAFTLang.Lexer.Text;
using SAFTLang.Modules;

namespace SAFTLang.SemanticAnalyzer;

internal sealed class SemanticAnalyzerState
{
    private readonly Stack<Dictionary<string, VariableSymbol>> _scopes = new();
    private readonly Dictionary<string, FunctionSymbol> _functions = new();

    private readonly Dictionary<Statement, LangType> _statementTypes = new();
    private readonly Dictionary<Expr, LangType> _expressionTypes = new();

    private readonly Dictionary<CallExpr, FunctionSymbol> _resolvedCalls = new();
    
    public Module? CurrentModule { get; set; }
    
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
        foreach (Dictionary<string, VariableSymbol> scope in _scopes)
        {
            if (scope.ContainsKey(name))
            {
                _diagnostics.ReportError(span, $"Variable '{name}' is already defined");
                
                return null;
            }
        }
        
        var symbol = new VariableSymbol(name, type, isConst);
        
        _scopes.Peek().Add(name, symbol);
        
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
    
    public void DeclareFunction(Module module, FunctionStatement function)
    {
        if (function.Name == "len")
        {
            _diagnostics.ReportError(function.Span, "Function len is already defined");
            return;
        }

        if (function.Name == "append")
        {
            _diagnostics.ReportError(function.Span, "Function append is already defined");
            return;
        }

        if (module.Imports.Any(import => import.LocalName == function.Name))
        {
            _diagnostics.ReportError(function.Span, $"Function '{function.Name}' is already defined by an import");
            return;
        }

        string qualifiedName = $"{module.FullName}::{function.Name}";
        
        if (_functions.ContainsKey(qualifiedName))
        {
            _diagnostics.ReportError(function.Span,
                $"Function '{function.Name}' is already defined"
            );
            
            return;
        }

        var symbol = new FunctionSymbol(
            function.Name,
            qualifiedName,
            function.Parameters
                .Select(parameter => parameter.Type)
                .ToList(),
            function.ReturnType
        );
        
        _functions.Add(qualifiedName, symbol);
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

    public FunctionSymbol? ResolveFunction(Expr callee, SourceSpan span)
    {
        return callee switch
        {
            IdentifierExpr identifierExpr =>
                ResolveUnqualifiedFunction(identifierExpr.Name, span),

            QualifiedNameExpr qualifiedNameExpr =>
                ResolveQualifiedFunction(qualifiedNameExpr, span),

            _ => null
        };
    }

    public FunctionSymbol? ResolveUnqualifiedFunction(string name, SourceSpan span)
    {
        if (CurrentModule is null)
        {
            _diagnostics.ReportError(span, $"Unknown function '{name}'");
            return null;
        }

        string localQualifiedName = $"{CurrentModule.FullName}::{name}";

        if (_functions.TryGetValue(localQualifiedName, out FunctionSymbol? localFunction))
        {
            return localFunction;
        }

        ImportBinding? import = CurrentModule.Imports.FirstOrDefault(binding =>
            binding.LocalName == name && binding.IsMemberImport);

        if (import is not null)
        {
            string qualifiedName = $"{import.ModuleName}::{import.MemberName}";

            if (_functions.TryGetValue(qualifiedName, out FunctionSymbol? importedFunction))
            {
                return importedFunction;
            }
            
            _diagnostics.ReportError(span, $"Module '{import.ModuleName}' has no function '{import.MemberName}'");
            return null;
        }
        
        _diagnostics.ReportError(span, $"Unknown function '{name}'");
        return null;
    }

    private FunctionSymbol? ResolveQualifiedFunction(QualifiedNameExpr name, SourceSpan span)
    {
        if (CurrentModule is null)
        {
            _diagnostics.ReportError(span, "Qualified function cannot be resolved outside a module");
            return null;
        }

        if (name.Parts.Count != 2)
        {
            _diagnostics.ReportError(span, "Qualified calls currently require 'module::function'");
            return null;
        }

        string alias = name.Parts[0];

        string functionName = name.Parts[1];

        ImportBinding? import =
            CurrentModule.Imports.FirstOrDefault(binding =>
                binding.LocalName == alias && !binding.IsMemberImport);

        if (import is null)
        {
            _diagnostics.ReportError(span, $"Unknown module alias '{alias}'");
            return null;
        }

        string qualifiedName = $"{import.ModuleName}::{functionName}";

        if (_functions.TryGetValue(qualifiedName, out FunctionSymbol? function))
        {
            return function;
        }
        
        _diagnostics.ReportError(span, $"Module '{import.ModuleName}' has no function '{functionName}'");
        return null;
    }

    public void SetResolvedFunction(
        CallExpr call,
        FunctionSymbol function)
    {
        _resolvedCalls[call] =
            function;
    }

    public FunctionSymbol GetResolvedFunction(
        CallExpr call)
    {
        if (_resolvedCalls.TryGetValue(
                call,
                out FunctionSymbol? function))
        {
            return function;
        }

        throw new InvalidOperationException(
            "No resolved function for call"
        );
    }
}