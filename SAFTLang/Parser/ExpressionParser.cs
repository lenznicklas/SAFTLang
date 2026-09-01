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
        Expr left = ParseCall();

        while (Current().Type == TokenType.Star || Current().Type == TokenType.Slash)
        {
            TokenType op = Current().Type;
            Advance();
            Expr right = ParseCall();
            
            left = new BinaryExpr(left, op, right, SourceSpan.Combine(left.Span, right.Span));
        }
        return left;
    }

    private Expr ParseCall()
    {
        Expr expression = ParsePrimary();
        while (Current().Type == TokenType.LParen)
        {
            expression = FinishCall(expression);
        }

        return expression;
    }

    private Expr FinishCall(Expr callee)
    {
        Consume(TokenType.LParen);
        
        var arguments = new List<Expr>();

        if (Current().Type != TokenType.RParen)
        {
            do
            {
                arguments.Add(ParseExpression());

                if (Current().Type != TokenType.Comma)
                {
                    break;
                }

                Consume(TokenType.Comma);
            } while (Current().Type != TokenType.RParen);
        }
        
        Token closingParen = Consume(TokenType.RParen);

        SourceSpan span = SourceSpan.Combine(callee.Span, closingParen.Span);

        return new CallExpr(callee, arguments, span);
    }

    private Expr ParsePrimary()
    {
        Token token = Current();

        if (token.Type == TokenType.Number)
        {
            Advance();
            return new IntegerExpr(
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

        if (token.Type == TokenType.LBracket)
        {
            return ParseArrayExpression();
        }

        _diagnostics.ReportError(
            token.Span,
            $"Expected expression, got " +
            $"{token.Type} ('{token.Value}')"
        );

        bool isExpressionBoundary =
            token.Type == TokenType.EOF ||
            token.Type == TokenType.Newline ||
            token.Type == TokenType.Semicolon ||
            token.Type == TokenType.RBrace;

        if (!isExpressionBoundary)
        {
            Advance();
        }

        return new ErrorExpr(token.Span);
    }

    private Expr ParseArrayExpression()
    {
        Token openingBracket = Consume(TokenType.LBracket);
        var elements = new List<Expr>();

        while (Current().Type != TokenType.RBracket && !IsAtEnd())
        {
            elements.Add(ParseExpression());

            if (Current().Type != TokenType.Comma)
            {
                break;
            }

            Consume(TokenType.Comma);
        }
        
        Token closingBracket = Consume(TokenType.RBracket);

        SourceSpan span = SourceSpan.Combine(openingBracket.Span, closingBracket.Span);
        
        return new ArrayExpr(elements, span);
    }

}