using SAFTLang.Diagnostics;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser;

internal sealed class ParserState
{
    private readonly IReadOnlyList<Token> _tokens;
    private readonly DiagnosticBag _diagnostics;
    
    public int Position { get; private set; }

    public ParserState(IReadOnlyList<Token> tokens, DiagnosticBag diagnostics)
    {
        _tokens = tokens;
        _diagnostics = diagnostics;
    }
    
    public Token Current => _tokens[Position];
    
    public bool IsAtEnd => Current.Type == TokenType.EOF;

    public Token Peek(int offset = 1)
    {
        int index = Position + offset;

        if (index >= _tokens.Count)
        {
            return _tokens[^1];
        }
        
        return _tokens[index];
    }

    public void Advance() => Position += IsAtEnd ? 0 : 1;

    public void SkipNewLines()
    {
        while (Current.Type == TokenType.Newline)
        {
            Advance();
        }
    }

    public void SynchronizeStatement()
    {
        while (!IsAtEnd &&
               Current.Type != TokenType.Newline &&
               Current.Type != TokenType.Semicolon &&
               Current.Type != TokenType.RBrace)
        {
            Advance();
        }

        if (Current.Type == TokenType.Semicolon || Current.Type == TokenType.Newline)
        {
            Advance();
        }
    }

    public Token Consume(TokenType type)
    {
        Token token = Current;

        if (token.Type == type)
        {
            Advance();
            return token;
        }

        _diagnostics.ReportError(
            token.Span,
            $"Expected {type}, got {token.Type} ('{token.Value}')"
        );

        return CreateMissingToken(type, token);
    }
    
    public void ConsumeStatementEnd()
    {
        if (Current.Type == TokenType.Semicolon)
        {
            Advance();

            if (Current.Type == TokenType.Newline)
            {
                Advance();
            }
            return;
        }

        if (Current.Type == TokenType.Newline ||
            Current.Type == TokenType.EOF ||
            Current.Type == TokenType.RBrace)
        {
            if (Current.Type == TokenType.Newline)
            {
                Advance();
            }
            return;
        }

        Token token = Current;
        
        _diagnostics.ReportError(
            token.Span,
            $"Expected ';' or newline, got " +
            $"{token.Type} ('{token.Value}')"
        );
        
        SynchronizeStatement();
    }


    private static Token CreateMissingToken(TokenType type, Token token)
    {
        var span = new SourceSpan(
            token.Span.Start,
            0,
            token.Span.Line,
            token.Span.Column
        );

        return new Token(type, "", span);
    }
    
}