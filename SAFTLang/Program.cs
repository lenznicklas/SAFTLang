using SAFTLang.Lexer;
using SAFTLang.Parser;
using SAFTLang.SemanticAnalyzer;
using SAFTLang.CodeGenerator;
using SAFTLang.AST;
using SAFTLang.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        string source = """
                        let x: int = 5
                        x = true
                        """;

        // Lexer
        var lexer = new Lexer(source);
        List<Token> tokens = lexer.Tokenize();

        // Parser
        var parser = new Parser(tokens);
        List<Statement> statements = parser.Parse();

        foreach (Diagnostic diagnostic in parser.Diagnostics)
        {
            Console.Error.WriteLine(diagnostic);
        }

        if (parser.HasErrors)
        {
            return;
        }

        // Semantic Analysis
        var analyzer = new SemanticAnalyzer();
        analyzer.Analyze(statements);

        if (analyzer.Diagnostics.Count > 0)
        {
            foreach (Diagnostic diagnostic in analyzer.Diagnostics)
            {
                Console.Error.WriteLine(diagnostic);
            }
            return;
        }

        // C Code Generation
        var generator = new CodeGenerator(analyzer);
        string cCode = generator.Generate(statements);

        Console.WriteLine(cCode);
    }
}