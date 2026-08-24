using SAFTLang.AST;
using SAFTLang.Lexer;

namespace SAFTLang.Parser;

public partial class Parser
{
    private readonly List<Token> _tokens;
    private int _position;
    
    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _position = 0;
    }

    public List<Statement> Parse()
    {
        var statements = new List<Statement>();

        while (!IsAtEnd())
        {
            if (Current().Type == TokenType.Newline)
            {
                Advance();
                continue;
            }
            
            statements.Add(ParseStatement());
        }
        return statements;
    }



}