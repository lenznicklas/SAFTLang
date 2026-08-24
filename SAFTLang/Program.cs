using SAFTLang.Lexer;
using SAFTLang.Parser;
using SAFTLang.SemanticAnalyzer;
using SAFTLang.CodeGenerator;
using SAFTLang.AST;

class Program
{
    static void Main(string[] args)
    {
        string source = """
                        let x = 10
                        let y = x + 10 +02
                        let int = true
                        let str = "string"
                        const pi = 5
                        """;

        // Lexer
        var lexer = new Lexer(source);
        List<Token> tokens = lexer.Tokenize();

        // Parser
        var parser = new Parser(tokens);
        List<Statement> statements = parser.Parse();

        // Semantic Analysis
        var analyzer = new SemanticAnalyzer();
        analyzer.Analyze(statements);

        // C Code Generation
        var generator = new CodeGenerator(analyzer);
        string cCode = generator.Generate(statements);

        Console.WriteLine(cCode);
    }
}