using SAFTLang.AST;
using SAFTLang.Lexer;

namespace SAFTLang.Parser;

public partial class Parser
{
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

        if (token.Type == TokenType.String)
        {
            Advance();
            return new StringExpr(token.Value);
        }

        if (token.Type == TokenType.Identifier)
        {
            Advance();
            return new IdentifierExpr(token.Value);
        }
        
        throw new Exception($"Expected expression, got {token.Type}");
    }

}