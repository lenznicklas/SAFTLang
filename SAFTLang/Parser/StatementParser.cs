using SAFTLang.Lexer;
using SAFTLang.AST;

namespace SAFTLang.Parser;

public partial class Parser
{
    private Statement ParseStatement()
    {

        return Current().Type switch
        {
            TokenType.Let => ParseLetStatement(),
            TokenType.Const => ParseConstStatement(),
            TokenType.If => ParseIfStatement(),
            _ => throw new Exception($"Unexpected token {Current().Type}"),
        };
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

    private Statement ParseIfStatement()
    {
        Consume(TokenType.If);

        Expr condition = ParseExpression();

        List<Statement> body = ParseBlock();

        return new IfStatement(condition, body);
    }

    private List<Statement> ParseBlock()
    {
        Consume(TokenType.LBrace);

        SkipNewLines();

        var statements = new List<Statement>();

        while (Current().Type != TokenType.RBrace && !IsAtEnd())
        {
            statements.Add(ParseStatement());
            SkipNewLines();
        }

        Consume(TokenType.RBrace);
        return statements;
    }

}