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
            case FunctionStatement stmt:
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

    private void AnalyzeLetStatement(LetStatement statement)
    {
        LangType? declaredType = statement.DeclaredType;
        LangType valueType = AnalyzeExpression(statement.Value, declaredType);

        if (valueType == LangType.Error)
        {
            return;
        }
        
        if (valueType == LangType.Void)
        {
            _diagnostics.ReportError(statement.Span,"Cannot assign type Void to a variable");
            return;
        }
        
        if (declaredType is null || declaredType == valueType)
        {
            DeclareVariable(statement.Name, valueType, isConst: false, statement.Span);
            _statementTypes[statement] = valueType;
        } 
        else if (declaredType != valueType)
        {
            _diagnostics.ReportError(statement.Span, $"Expected {valueType} to be declared as {valueType}, not {declaredType}");
        }
    }

    private void AnalyzeConstStatement(ConstStatement statement)
    {
        LangType? declaredType = statement.DeclaredType;
        LangType valueType = AnalyzeExpression(statement.Value, declaredType);

        if (valueType == LangType.Error)
        {
            return;
        }
        
        if (valueType == LangType.Void)
        {
            _diagnostics.ReportError(statement.Span, "Cannot assign type Void to a variable");
            return;
        }

        if (declaredType is null || declaredType == valueType)
        {
            DeclareVariable(statement.Name, valueType, isConst: true, statement.Span);
            _statementTypes[statement] = valueType;
        }
        else if (declaredType != valueType)
        {
            _diagnostics.ReportError(statement.Span, $"Expected {valueType} to be declared as {valueType}, not {declaredType}");
        }
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

        LangType valueType = AnalyzeExpression(statement.Value, symbol.Type);

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

    private void AnalyzeFunctionStatement(
        FunctionStatement functionStatement)
    {
        FunctionStatement? previousFunction = _currentFunction;
        
        _currentFunction = functionStatement;
        
        BeginScope();

        try
        {
            foreach (Parameter parameter in functionStatement.Parameters)
            {
                DeclareVariable(
                    parameter.Name,
                    parameter.Type,
                    isConst: false,
                    parameter.Span
                );
            }

            foreach (Statement statement in functionStatement.Body.Statements)
            {
                AnalyzeStatement(statement);
            }

            if (functionStatement.ReturnType != LangType.Void &&
                !AlwaysReturns(functionStatement.Body))
            {
                _diagnostics.ReportError(
                    functionStatement.Span,
                    $"'{functionStatement.Name}' has to return a value of type {functionStatement.ReturnType}"
                );
            }
        }
        finally
        {
            EndScope();

            _currentFunction = previousFunction;
        }
    }

    private void AnalyzeReturnStatement(ReturnStatement statement)
    {
        if (_currentFunction is null)
        {
            _diagnostics.ReportError(
                statement.Span,
                "Return statement is only allowed inside a function"
            );
            
            return;
        }

        LangType expectedType = _currentFunction.ReturnType;

        if (expectedType == LangType.Void)
        {
            if (statement.Value is not null)
            {
                AnalyzeExpression(statement.Value);
                _diagnostics.ReportError(
                    statement.Span,
                    $"Void func '{_currentFunction.Name}' cannot return a value"
                );
            }
            
            return;
        }

        if (statement.Value is null)
        {
            _diagnostics.ReportError(
                statement.Span,
                $"Function '{_currentFunction.Name}' has to return a value of type {expectedType}"
            );
            
            return;
        }

        LangType actualType = AnalyzeExpression(statement.Value, expectedType);

        if (actualType == LangType.Error)
        {
            return;
        }

        if (actualType != expectedType)
        {
            _diagnostics.ReportError(
                statement.Span,
                $"Expected func '{_currentFunction.Name}' to return a value of {expectedType} not {actualType}"
            );
        }
    }
    
}