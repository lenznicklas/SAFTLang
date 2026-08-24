using SAFTLang.Lexer;
using SAFTLang.AST;

namespace SAFTLang.Parser;

public partial class Parser
{
    private Statement ParseStatement()
    {
        if (Current().Type == TokenType.Let)
        {
            return ParseLetStatement();
        }
        
        throw new Exception($"Unexpected token {Current().Type}");
    }

    private Statement ParseLetStatement()
    {
        Consume(TokenType.Let);
        
        Token name = Consume(TokenType.Identifier);

        Consume(TokenType.Equals);

        Expr value = ParseExpression();

        ConsumeStatementEnd();

        return new LetStatement(name.Value, value);
    }

}