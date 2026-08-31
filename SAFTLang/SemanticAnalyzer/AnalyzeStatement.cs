using SAFTLang.AST;
using SAFTLang.SemanticAnalyzer.Symbols;

namespace SAFTLang.SemanticAnalyzer;

public partial class SemanticAnalyzer
{
    private void AnalyzeStatement(Statement statement)
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
                AnalyzeExpression(stmt.Expression);
                break;
            default:
                _diagnostics.ReportError(
                    statement.Span,
                    $"Unknown statement {statement.GetType().Name}"
                );
                break;
        }
    }

    private void AnalyzeLetStatement(LetStatement statement)
    {
        LangType type= AnalyzeExpression(statement.Value);
        DeclareVariable(statement.Name, type, isConst: false, statement.Span);
        _statementTypes[statement] = type;
    }

    private void AnalyzeConstStatement(ConstStatement statement)
    {
        LangType type = AnalyzeExpression(statement.Value);
        DeclareVariable(statement.Name, type, isConst:true, statement.Span);
        _statementTypes[statement] = type;
    }

    private void AnalyzeIfStatement(IfStatement statement)
    {
        LangType conditionType = AnalyzeExpression(statement.Condition);

        if (conditionType != LangType.Bool &&
            conditionType != LangType.Error)
        {
            _diagnostics.ReportError(
                statement.Condition.Span,
                $"If condition must be Bool, got {conditionType}"
            );
        }
        
        AnalyzeBlockStatement(statement.thenBody);

        if (statement.elseBody is not null)
        {
            AnalyzeBlockStatement(statement.elseBody);
        }
    }

    private void AnalyzeBlockStatement(BlockStatement block)
    {
        BeginScope();

        try
        {
            foreach (Statement statement in block.Statements)
            {
                AnalyzeStatement(statement);
            }
        }
        finally
        {
            EndScope();
        }
    }

    private void AnalyzeAssignmentStatement(AssignmentStatement statement)
    {
        VariableSymbol? symbol = ResolveVariable(statement.Name, statement.Span);

        if (symbol is null)
        {
            return;
        }
        
        if (symbol.IsConst)
        {
            _diagnostics.ReportError(
                statement.Span,
                $"Can't assign {symbol.Name} to type const"
            );
        }

        LangType valueType = AnalyzeExpression(statement.Value);

        if (valueType == LangType.Error)
        {
            return;
        }

        if (valueType != symbol.Type)
        {
            _diagnostics.ReportError(
                statement.Value.Span,
                $"Can't assign {valueType} to {symbol.Name} of type {symbol.Type}"
            );
        }
    }
    
}