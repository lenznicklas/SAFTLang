using SAFTLang.Lexer;

namespace SAFTLang.Parser;

public partial class Parser
{
    private void ConsumeStatementEnd()
    {
        if (Current().Type == TokenType.Semicolon)
        {
            Advance();

            if (Current().Type == TokenType.Newline)
            {
                Advance();
            }
            return;
        }

        if (Current().Type == TokenType.Newline)
        {
            Advance();
            return;
        }

        if (Current().Type == TokenType.EOF)
        {
            return;
        }

        throw new Exception("Expected ';' or newline");
    }

    private Token Consume(TokenType type)
    {
        Token token = Current();

        if (token.Type != type)
        {
            throw new Exception($"Unexpected token, expected {type}, got {token.Type}");
        }
        
        Advance();
        return token;
    }
    private Token Current()
    {
        return _tokens[_position];
    }
    
    private void Advance()
    {
        _position++;
    }

    private bool IsAtEnd()
    {
        return Current().Type == TokenType.EOF;
    }

    private void SkipNewLines()
    {
        while (Current().Type == TokenType.Newline)
        {
            Advance();
        }
    }

}