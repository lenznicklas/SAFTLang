using SAFTLang.AST.Expressions;
using SAFTLang.AST.Statements;
using SAFTLang.Diagnostics;
using SAFTLang.SemanticAnalyzer.AnalyzeExpressions;
using SAFTLang.SemanticAnalyzer.ControlFlow;

namespace SAFTLang.SemanticAnalyzer.AnalyzeStatements;

internal sealed partial class StatementAnalyzer
{
    private readonly SemanticAnalyzerState _state;
    private readonly ExpressionAnalyzer _expressionAnalyzer;
    private readonly ControlFlowAnalyzer _controlFlow;
    private readonly DiagnosticBag _diagnostics;

    public StatementAnalyzer(SemanticAnalyzerState state, ExpressionAnalyzer expressionAnalyzer,
        ControlFlowAnalyzer controlFlow, DiagnosticBag diagnostics)
    {
        _state = state;
        _expressionAnalyzer = expressionAnalyzer;
        _controlFlow = controlFlow;
        _diagnostics = diagnostics;
    }
    
    public void AnalyzeStatement(Statement statement)
    {
        switch (statement)
        {
            case LetStatement stmt:
                AnalyzeLetStatement(stmt);
                break;
            case ConstStatement stmt:
                AnalyzeConstStatement(stmt);
                break;
            case IfStatement stmt:
                AnalyzeIfStatement(stmt);
                break;
            case BlockStatement stmt:
                AnalyzeBlockStatement(stmt);
                break;
            case AssignmentStatement stmt:
                AnalyzeAssignmentStatement(stmt);
                break;
            case ExpressionStatement stmt:
                _expressionAnalyzer.AnalyzeExpression(stmt.Expression);
                break;
            case FunctionStatement stmt:
                if (_state.CurrentFunction is not null)
                {
                    _diagnostics.ReportError(stmt.Span, "Cannot create a function inside of a function");
                }
                AnalyzeFunctionStatement(stmt);
                break;
            case ReturnStatement stmt:
                AnalyzeReturnStatement(stmt);
                break;
            default:
                _diagnostics.ReportError(
                    statement.Span,
                    $"Unknown statement {statement.GetType().Name}"
                );
                break;
        }
    }
    
    private IdentifierExpr? GetRootIdentifier(Expr expr)
    {
        return expr switch
        {
            IdentifierExpr identifierExpr => identifierExpr,

            IndexExpr indexExpr => GetRootIdentifier(indexExpr.Target),

            _ => null
        };
    }


}