using SAFTLang.AST.Statements;
using SAFTLang.AST.Types;
using SAFTLang.Diagnostics;
using SAFTLang.Lexer.Text;
using SAFTLang.Modules;

namespace SAFTLang.SemanticAnalyzer.ProgramValidation;

internal sealed class ProgramValidator
{
    private readonly DiagnosticBag _diagnostics;

    public ProgramValidator(
        DiagnosticBag diagnostics)
    {
        _diagnostics = diagnostics;
    }

    public void ValidateMain(
        Module entryModule)
    {
        FunctionStatement? main =
            entryModule.Statements
                .OfType<FunctionStatement>()
                .FirstOrDefault(function =>
                    function.Name == "main");

        if (main is null)
        {
            SourceSpan span =
                entryModule.Statements.Count > 0
                    ? entryModule.Statements[0].Span
                    : new SourceSpan(
                        0,
                        0,
                        1,
                        1
                    );

            _diagnostics.ReportError(
                span,
                "Entry module must define a main function"
            );

            return;
        }

        if (main.Parameters.Count != 0 ||
            main.ReturnType != LangType.Void)
        {
            _diagnostics.ReportError(
                main.Span,
                "Main function must have the signature " +
                "'func main() void'"
            );
        }
    }
}