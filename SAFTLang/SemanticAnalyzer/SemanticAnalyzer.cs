using SAFTLang.Lexer;
using SAFTLang.AST;

namespace SAFTLang.SemanticAnalyzer;

public partial class SemanticAnalyzer
{
    private readonly Dictionary<string, LangType> _variables = new();
    
    public LangType GetVariableType(string name)
    {
        return _variables[name];
    }

    public void Analyze(List<Statement> statements)
    {
        foreach (var statement in statements)
        {
            AnalyzeStatement(statement);
        }
    }


}