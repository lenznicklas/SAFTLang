using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseExpressions;

internal sealed partial class ExpressionParser
{
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

        if (token.Type == TokenType.Char)
        {
            _state.Advance();
            return new CharExpr(
                token.Value[0],
                token.Span
            );
        }

        if (token.Type == TokenType.Identifier)
        {
            _state.Advance();

            var parts = new List<string>
            {
                token.Value
            };

            SourceSpan lastSpan = token.Span;

            while (_state.Current.Type == TokenType.DoubleColon)
            {
                _state.Consume(TokenType.DoubleColon);

                Token part = _state.Consume(TokenType.Identifier);
                parts.Add(part.Value);
                
                lastSpan = part.Span;
            }

            if (parts.Count == 1)
            {
                return new IdentifierExpr(
                    token.Value,
                    token.Span
                );
            }

            return new QualifiedNameExpr(parts, SourceSpan.Combine(token.Span, lastSpan));

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

}