namespace SAFTLang.Parser;

using SAFTLang.Lexer;

public abstract record Expr;

public record NumberExpr(string Value) : Expr;
public record IdentifierExpr(string Name) : Expr;
public record BinaryExpr(Expr Left, TokenType Operator, Expr Right) : Expr;
public record BoolExpr(bool Value) : Expr;

public abstract record Statement;

public record LetStatement(string Name, Expr Value) : Statement;


public class Parser
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

    private Expr ParseExpression()
    {
        return ParseAddition();
    }

    private Expr ParseAddition()
    {
        Expr left = ParseMultiplication();
        
        while (Current().Type == TokenType.Plus || Current().Type == TokenType.Minus)
        {
            TokenType op = Current().Type;
            Advance();

            Expr right = ParseMultiplication();
            left = new BinaryExpr(left, op, right);
        }
        return left;
    }

    private Expr ParseMultiplication()
    {
        Expr left = ParsePrimary();

        while (Current().Type == TokenType.Star || Current().Type == TokenType.Slash)
        {
            TokenType op = Current().Type;
            Advance();
            Expr right = ParsePrimary();
            
            left = new BinaryExpr(left, op, right);
        }
        return left;
    }

    private Expr ParsePrimary()
    {
        Token token = Current();

        if (token.Type == TokenType.Number)
        {
            Advance();
            return new NumberExpr(token.Value);
        }

        if (token.Type == TokenType.True)
        {
            Advance();
            return new BoolExpr(true);
        }

        if (token.Type == TokenType.False)
        {
            Advance();
            return new BoolExpr(false);
        }

        if (token.Type == TokenType.Identifier)
        {
            Advance();
            return new IdentifierExpr(token.Value);
        }
        
        throw new Exception($"Expected expression, got {token.Type}");
    }

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
}