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

        if (Current().Type == TokenType.Const)
        {
            return ParseConstStatement();
        }
        
        throw new Exception($"Unexpected token {Current().Type}");
    }

    private Statement ParseLetStatement()
    {
        Consume(TokenType.Let);
        
        Token name = Consume(TokenType.Identifier);

        Consume(TokenType.Equal);

        Expr value = ParseExpression();

        ConsumeStatementEnd();

        return new LetStatement(name.Value, value);
    }

    private Statement ParseConstStatement()
    {
        Consume(TokenType.Const);

        Token name = Consume(TokenType.Identifier);

        Consume(TokenType.Equal);

        Expr value = ParseExpression();
        
        ConsumeStatementEnd();
        
        return new ConstStatement(name.Value, value);
    }

}