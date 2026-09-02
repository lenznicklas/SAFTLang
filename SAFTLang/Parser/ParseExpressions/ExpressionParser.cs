using SAFTLang.AST;
using SAFTLang.Diagnostics;
using SAFTLang.Lexer.TokenAndKeywords;
using SAFTLang.Lexer.Text;

namespace SAFTLang.Parser.ParseExpressions;

internal sealed class ExpressionParser
{
    private readonly ParserState _state;
    private readonly DiagnosticBag _diagnostics;

    public ExpressionParser(ParserState state, DiagnosticBag diagnostics)
    {
        _state = state;
        _diagnostics = diagnostics;
    }

    public Expr ParseExpression()
    {
        return ParseEquality();
    }
    
    private Expr ParseEquality()
    {
        Expr left = ParseComparison();

        while (_state.Current.Type == TokenType.EqualEqual ||
               _state.Current.Type == TokenType.NotEqual)
        {
            TokenType op = _state.Current.Type;
            _state.Advance();

            Expr right = ParseComparison();
            
            left = new BinaryExpr(left, op, right, SourceSpan.Combine(left.Span, right.Span));
        }
        
        return left;
    }
    
    private Expr ParseComparison()
    {
        Expr left = ParseAddition();

        while (_state.Current.Type == TokenType.Less ||
               _state.Current.Type == TokenType.LessEqual ||
               _state.Current.Type == TokenType.Greater ||
               _state.Current.Type == TokenType.GreaterEqual)
        {
            TokenType op = _state.Current.Type;
            _state.Advance();
            Expr right = ParseAddition();
            left = new BinaryExpr(left, op, right, SourceSpan.Combine(left.Span, right.Span));
        }
        
        return left;
    }

    private Expr ParseAddition()
    {
        Expr left = ParseMultiplication();
        
        while (_state.Current.Type == TokenType.Plus || _state.Current.Type == TokenType.Minus)
        {
            TokenType op = _state.Current.Type;
            _state.Advance();

            Expr right = ParseMultiplication();
            left = new BinaryExpr(left, op, right, SourceSpan.Combine(left.Span, right.Span));
        }
        return left;
    }

    private Expr ParseMultiplication()
    {
        Expr left = ParsePostfix();

        while (_state.Current.Type == TokenType.Star || _state.Current.Type == TokenType.Slash)
        {
            TokenType op = _state.Current.Type;
            _state.Advance();
            Expr right = ParsePostfix();
            
            left = new BinaryExpr(left, op, right, SourceSpan.Combine(left.Span, right.Span));
        }
        return left;
    }

    private Expr ParsePostfix()
    {
        Expr expr = ParsePrimary();

        while (true)
        {
            if (_state.Current.Type == TokenType.LParen)
            {
                expr = FinishCall(expr);
                continue;
            }

            if (_state.Current.Type == TokenType.LBracket)
            {
                expr = FinishIndex(expr);
                continue;
            }
            
            break;
        }

        return expr;
    }
    
    private Expr ParseCall()
    {
        Expr expression = ParsePrimary();
        while (_state.Current.Type == TokenType.LParen)
        {
            expression = FinishCall(expression);
        }

        return expression;
    }

    private Expr ParsePrimary()
    {
        Token token = _state.Current;

        if (token.Type == TokenType.Number)
        {
            _state.Advance();
            return new IntegerExpr(
                token.Value,
                token.Span
            );
        }

        if (token.Type == TokenType.True)
        {
            _state.Advance();
            return new BoolExpr(
                true,
                token.Span
            );
        }

        if (token.Type == TokenType.False)
        {
            _state.Advance();
            return new BoolExpr(
                false,
                token.Span
            );
        }

        if (token.Type == TokenType.String)
        {
            _state.Advance();
            return new StringExpr(
                token.Value,
                token.Span
            );
        }

        if (token.Type == TokenType.Identifier)
        {
            _state.Advance();
            return new IdentifierExpr(
                token.Value,
                token.Span
            );
        }

        if (token.Type == TokenType.LParen)
        {
            _state.Advance();
            Expr expression = ParseExpression();

            _state.Consume(TokenType.RParen);
            
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
            _state.Advance();
        }

        return new ErrorExpr(token.Span);
    }
    
    private Expr ParseArrayExpression()
    {
        Token openingBracket = _state.Consume(TokenType.LBracket);
        var elements = new List<Expr>();

        while (_state.Current.Type != TokenType.RBracket && !_state.IsAtEnd)
        {
            elements.Add(ParseExpression());

            if (_state.Current.Type != TokenType.Comma)
            {
                break;
            }

            _state.Consume(TokenType.Comma);
        }
        
        Token closingBracket = _state.Consume(TokenType.RBracket);

        SourceSpan span = SourceSpan.Combine(openingBracket.Span, closingBracket.Span);
        
        return new ArrayExpr(elements, span);
    }
    
    private Expr FinishCall(Expr callee)
    {
        _state.Consume(TokenType.LParen);
        
        var arguments = new List<Expr>();

        if (_state.Current.Type != TokenType.RParen)
        {
            do
            {
                arguments.Add(ParseExpression());

                if (_state.Current.Type != TokenType.Comma)
                {
                    break;
                }

                _state.Consume(TokenType.Comma);
            } while (_state.Current.Type != TokenType.RParen);
        }
        
        Token closingParen = _state.Consume(TokenType.RParen);

        SourceSpan span = SourceSpan.Combine(callee.Span, closingParen.Span);

        return new CallExpr(callee, arguments, span);
    }

    private Expr FinishIndex(Expr target)
    {
        _state.Consume(TokenType.LBracket);

        Expr index = ParseExpression();

        Token closingBracket = _state.Consume(TokenType.RBracket);

        SourceSpan span = SourceSpan.Combine(target.Span, closingBracket.Span);

        return new IndexExpr(target, index, span);
    }


}