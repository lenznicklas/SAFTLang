using SAFTLang.AST;

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
        DeclareVariable(statement.Name, type);
        _statementTypes[statement] = type;
    }

    private void AnalyzeConstStatement(ConstStatement statement)
    {
        LangType type = AnalyzeExpression(statement.Value);
        DeclareVariable(statement.Name, type);
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

        try
        {
            foreach (Statement bodyStatement in statement.Body)
            {
                AnalyzeStatement(bodyStatement);
            }
        }
        finally
        {
            EndScope();
        }
    }

}