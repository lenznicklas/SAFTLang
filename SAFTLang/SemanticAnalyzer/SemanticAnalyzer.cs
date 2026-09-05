using SAFTLang.AST.Expressions;
using SAFTLang.AST.Statements;
using SAFTLang.AST.Types;
using SAFTLang.Diagnostics;
using SAFTLang.Modules;
using SAFTLang.SemanticAnalyzer.AnalyzeExpressions;
using SAFTLang.SemanticAnalyzer.AnalyzeStatements;
using SAFTLang.SemanticAnalyzer.ControlFlow;
using SAFTLang.SemanticAnalyzer.ProgramValidation;
using SAFTLang.SemanticAnalyzer.Symbols;

namespace SAFTLang.SemanticAnalyzer;

public sealed class SemanticAnalyzer
{
    private readonly SemanticAnalyzerState _state;
    private readonly StatementAnalyzer _statementAnalyzer;
    private readonly ProgramValidator _programValidator;
    
    private readonly DiagnosticBag _diagnostics = new();
    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics.Diagnostics;
    
    public SemanticAnalyzer()
    {
        _state = new SemanticAnalyzerState(_diagnostics);

        var expressions = new ExpressionAnalyzer(_state, _diagnostics);
        var controlFlow = new ControlFlowAnalyzer();
        
        _statementAnalyzer = new StatementAnalyzer(_state, expressions, controlFlow, _diagnostics);
        _programValidator = new ProgramValidator(_diagnostics);
    }
    
    public void Analyze(List<Statement> statements)
    {
        var module = new Module(["project", "main"], statements);
        AnalyzeModules([module], module);
        foreach (Statement statement in statements)
        {
            if (statement is not FunctionStatement function)
            {
                _diagnostics.ReportError(statement.Span, "Only function declarations are allowed at top level");
            }
            else
            {
                _state.DeclareFunction(module, function);
            }
        }

        _programValidator.ValidateMain(module);

        foreach (Statement statement in statements)
        {
            _statementAnalyzer.AnalyzeStatement(statement);
        }
    }

    public LangType GetStatementType(Statement statement)
    {
        return _state.GetStatementType(statement);
    }
    
    public LangType GetExpressionType(Expr expr)
    {
        return _state.GetExpressionType(expr);
    }

    public void AnalyzeModules(IReadOnlyList<Module> modules, Module entryModule)
    {
        foreach (Module module in modules)
        {
            foreach (Statement statement in module.Statements)
            {
                switch (statement)
                {
                    case ImportStatement:
                        break;
                    
                    case FunctionStatement func:
                        _state.DeclareFunction(module, func);
                        break;
                    
                    default:
                        _diagnostics.ReportError(
                            statement.Span,
                            "Only imports and function declarations are allowed at top level"
                        );
                        break;
                }
            }
        }
        
        _programValidator.ValidateMain(entryModule);

        foreach (Module module in modules)
        {
            _state.CurrentModule = module;
            foreach (FunctionStatement function in module.Statements.OfType<FunctionStatement>())
            {
                _statementAnalyzer.AnalyzeStatement(function);
            }
        }
        
        _state.CurrentModule = null;
    }
    
    public FunctionSymbol GetResolvedFunction(
        CallExpr call)
    {
        return _state.GetResolvedFunction(
            call
        );
    }


}