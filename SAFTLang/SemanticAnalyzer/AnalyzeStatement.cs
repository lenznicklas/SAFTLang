using SAFTLang.AST;
using SAFTLang.SemanticAnalyzer.Symbols;

namespace SAFTLang.SemanticAnalyzer;

public partial class SemanticAnalyzer
{
    private void AnalyzeStatement(Statement statement)
    {
        switch (statement)
        {
            case LetStatement let:
                AnalyzeLetStatement(let);
                break;
            case ConstStatement _const:
                AnalyzeConstStatement(_const);
                break;
            case IfStatement _if:
                AnalyzeIfStatement(_if);
                break;
            default:
                throw new Exception($"Unknown statement {statement.GetType().Name}");
        }
    }

    private void AnalyzeLetStatement(LetStatement statement)
    {
        LangType type= AnalyzeExpression(statement.Value);
        DeclareVariable(statement.Name, type, isConst: false);
        _statementTypes[statement] = type;
    }

    private void AnalyzeConstStatement(ConstStatement statement)
    {
        LangType type = AnalyzeExpression(statement.Value);
        DeclareVariable(statement.Name, type, isConst:true);
        _statementTypes[statement] = type;
    }

    private void AnalyzeIfStatement(IfStatement statement)
    {
        LangType conditionType = AnalyzeExpression(statement.Condition);

        if (conditionType != LangType.Bool)
        {
            throw new Exception(
                $"If condition must be Bool, got {conditionType}"
            );
        }

        BeginScope();

        AnalyzeBlockStatement(statement.Body);
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
        VariableSymbol symbol = ResolveVariable(statement.Name);

        if (symbol.IsConst)
        {
            throw new Exception($"Can't assign {symbol.Name} to const");
        }

        LangType valueType = AnalyzeExpression(statement.Value);

        if (valueType != symbol.Type)
        {
            throw new Exception($"Can't assign {symbol.Name} to type {valueType}");
        }
    }
    
}