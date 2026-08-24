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
                        let number = 10
                        let secondNumber = 5
                        const limit = 20
                        
                        let addition = number + secondNumber
                        let subtraction = number - secondNumber
                        let multiplication = number * secondNumber
                        let division = number / secondNumber
                        
                        let precedence = 2 + 3 * 4
                        let parentheses = (2 + 3) * 4
                        let nestedParentheses = ((2 + 3) * (4 + 1))
                        
                        let isGreater = number > secondNumber
                        let isLess = number < limit
                        let isGreaterOrEqual = number >= 10
                        let isLessOrEqual = secondNumber <= 5
                        let numbersEqual = number == 10
                        let numbersDifferent = number != secondNumber
                        
                        let enabled = true
                        let disabled = false
                        let booleansEqual = enabled == true
                        let booleansDifferent = enabled != disabled
                        
                        let multilineExpression = (
                            number
                            + secondNumber
                            * 2
                        )
                        
                        if number > secondNumber {
                            let insideIf = true
                            const insideLimit = 100
                        
                            let calculated = number + insideLimit
                        
                            if insideIf {
                                let nestedValue = (calculated + 10) / 2
                                let nestedComparison = nestedValue >= limit
                            }
                        }
                        
                        let afterIf = number + 1
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