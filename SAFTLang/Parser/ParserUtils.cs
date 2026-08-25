using System.Diagnostics.CodeAnalysis;
using SAFTLang.Lexer;
using SAFTLang.Lexer.Text;

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

        if (Current().Type == TokenType.Newline ||
            Current().Type == TokenType.EOF ||
            Current().Type == TokenType.RBrace)
        {
            if (Current().Type == TokenType.Newline)
            {
                Advance();
            }
            return;
        }

        Token token = Current();
        
        _diagnostics.ReportError(
            token.Span,
            $"Expected ';' or newline, got " +
            $"{token.Type} ('{token.Value}')"
            );
        
        SynchronizeStatement();
    }

    private Token Consume(TokenType type)
    {
        Token token = Current();

        if (token.Type == type)
        {
            Advance();
            return token;
        }

        if (token.Type != type)
        {
            _diagnostics.ReportError(
                token.Span,
                $"Expected {type}, got " +
                $"{token.Type} ('{token.Value}')"
            );
        }

        return CreateMissingToken(
            type,
            token
        );
    }

    private Token CreateMissingToken(TokenType type, Token token)
    {
        var span = new SourceSpan(
            token.Span.Start,
            0,
            token.Span.Line,
            token.Span.Column
        );

        return new Token(
            type,
            "",
            span
        );
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

    private void SynchronizeStatement()
    {
        while (!IsAtEnd() &&
               Current().Type != TokenType.Newline &&
               Current().Type != TokenType.Semicolon &&
               Current().Type != TokenType.RBrace)
        {
            Advance();
        }

        if (Current().Type == TokenType.Semicolon ||
            Current().Type == TokenType.Newline)
        {
            Advance();
        }
    }

}