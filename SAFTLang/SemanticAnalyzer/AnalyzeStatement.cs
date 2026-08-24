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
            default:
                throw new Exception($"Unknown statement {statement.GetType().Name}");
        }
    }

    private void AnalyzeLetStatement(LetStatement statement)
    {
        if (_variables.ContainsKey(statement.Name))
        {
            throw new Exception($"Variable {statement.Name} already defined");
        }

        LangType inferredType = AnalyzeExpression(statement.Value);
        
        _variables.Add(statement.Name, inferredType);
    }
    
    private void AnalyzeConstStatement(ConstStatement statement)
    {
        if (_variables.ContainsKey(statement.Name))
        {
            throw new Exception($"Variable {statement.Name} already defined");
        }

        LangType inferredType = AnalyzeExpression(statement.Value);
        
        _variables.Add(statement.Name, inferredType);
    }

}