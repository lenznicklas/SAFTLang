using SAFTLang.AST;
using SAFTLang.Diagnostics;
using SAFTLang.Lexer;

namespace SAFTLang;

public static class CompilerDriver
{
    public static string? CompileFile(string path)
    {
        string source;

        try
        {
            source = File.ReadAllText(path);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(
                $"Could not read '{path}': " +
                e.Message);
            
            return null;
        }

        return Compile(source);
    }

    public static string? Compile(string source)
    {
        List<Token> tokens;

        // Lexer
        try
        {
            var lexer = new Lexer.Lexer(source);

            tokens = lexer.Tokenize();
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(
                $"Lexer error: {e.Message}"
            );
            return null;
        }
        
        // Parser
        var parser = new Parser.Parser(tokens);

        List<Statement> statements = parser.Parse();

        PrintDiagnostics(parser.Diagnostics);

        if (parser.HasErrors)
        {
            return null;
        }
        
        // Semantic Analyzer
        var analyzer = new SemanticAnalyzer.SemanticAnalyzer();
        
        analyzer.Analyze(statements);
        
        PrintDiagnostics(analyzer.Diagnostics);

        if (analyzer.Diagnostics.Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error))
        {
            return null;
        }
        
        // Code Generator
        var generator = new CodeGenerator.CodeGenerator(analyzer);

        return generator.Generate(statements);
    }
    
    private static void PrintDiagnostics(
        IEnumerable<Diagnostic> diagnostics)
    {
        foreach (Diagnostic diagnostic
                 in diagnostics)
        {
            Console.Error.WriteLine(
                diagnostic
            );
        }
    }
}