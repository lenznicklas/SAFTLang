using SAFTLang.AST;
using SAFTLang.SemanticAnalyzer.Symbols;

namespace SAFTLang.SemanticAnalyzer;

public partial class SemanticAnalyzer
{
    private void ValidateMain(List<Statement> statements)
    {
        if (!_functions.TryGetValue("main", out FunctionSymbol? main))
        {
            if (statements.Count > 0){
                _diagnostics.ReportError(statements[0].Span, "Program must define a main function");
            }
            return;
        }
        
        if (main.ParameterTypes.Count != 0 || 
            main.ReturnType != LangType.Void)
        {
            FunctionStatement? mainStatement = statements
                .OfType<FunctionStatement>()
                .FirstOrDefault(statement => statement.Name == "main"
                );

            if (mainStatement is not null)
            {
                _diagnostics.ReportError(
                    mainStatement.Span,
                    "Main function must have the signature 'func main() void'"
                );
            }
        }
    }
}