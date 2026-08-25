using SAFTLang.AST;
using SAFTLang.Lexer;
using SAFTLang.Lexer.Text;

namespace SAFTLang.Parser;

public partial class Parser
{
    private Expr ParseExpression()
    {
        return ParseEquality();
    }

    private Expr ParseEquality()
    {
        Expr left = ParseComparison();

        while (Current().Type == TokenType.EqualEqual ||
               Current().Type == TokenType.NotEqual)
        {
            TokenType op = Current().Type;
            Advance();

            Expr right = ParseComparison();
            
            left = new BinaryExpr(left, op, right, SourceSpan.Combine(left.Span, right.Span));
        }
        
        return left;
    }

    private Expr ParseComparison()
    {
        Expr left = ParseAddition();

        while (Current().Type == TokenType.Less ||
               Current().Type == TokenType.LessEqual ||
               Current().Type == TokenType.Greater ||
               Current().Type == TokenType.GreaterEqual)
        {
            TokenType op = Current().Type;
            Advance();
            Expr right = ParseAddition();
            left = new BinaryExpr(left, op, right, SourceSpan.Combine(left.Span, right.Span));
        }
        
        return left;
    }

    private Expr ParseAddition()
    {
        Expr left = ParseMultiplication();
        
        while (Current().Type == TokenType.Plus || Current().Type == TokenType.Minus)
        {
            TokenType op = Current().Type;
            Advance();

            Expr right = ParseMultiplication();
            left = new BinaryExpr(left, op, right, SourceSpan.Combine(left.Span, right.Span));
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
            
            left = new BinaryExpr(left, op, right, SourceSpan.Combine(left.Span, right.Span));
        }
        return left;
    }

    private Expr ParsePrimary()
    {
        Token token = Current();

        if (token.Type == TokenType.Number)
        {
            Advance();
            return new NumberExpr(
                token.Value,
                token.Span
            );
        }

        if (token.Type == TokenType.True)
        {
            Advance();
            return new BoolExpr(
                true,
                token.Span
            );
        }

        if (token.Type == TokenType.False)
        {
            Advance();
            return new BoolExpr(
                false,
                token.Span
            );
        }

        if (token.Type == TokenType.String)
        {
            Advance();
            return new StringExpr(
                token.Value,
                token.Span
            );
        }

        if (token.Type == TokenType.Identifier)
        {
            Advance();
            return new IdentifierExpr(
                token.Value,
                token.Span
            );
        }

        if (token.Type == TokenType.LParen)
        {
            Advance();
            Expr expression = ParseExpression();

            Consume(TokenType.RParen);
            
            return expression;
        }
        
        throw new Exception($"{token.Span}: Expected expression, got {token.Type}");
    }

}