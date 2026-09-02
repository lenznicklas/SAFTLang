using SAFTLang.AST;
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

}