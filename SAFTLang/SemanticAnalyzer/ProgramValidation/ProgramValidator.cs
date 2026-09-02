using SAFTLang.AST.Statements;
using SAFTLang.AST.Types;
using SAFTLang.Diagnostics;
using SAFTLang.Lexer.Text;
using SAFTLang.SemanticAnalyzer.Symbols;

namespace SAFTLang.SemanticAnalyzer.ProgramValidation;

internal sealed class ProgramValidator
{
    private readonly SemanticAnalyzerState _state;
    private readonly DiagnosticBag _diagnostics;

    public ProgramValidator(SemanticAnalyzerState state, DiagnosticBag diagnostics)
    {
        _state = state;
        _diagnostics = diagnostics;
    }
    
    public void ValidateMain(List<Statement> statements)
    {
        if (!_state.TryGetFunction("main", out FunctionSymbol? main))
        {
            SourceSpan span = statements.Count > 0
                ? statements[0].Span
                : new SourceSpan(0, 0, 1, 1);
            
            _diagnostics.ReportError(span, "Program must define a main function");
            
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