using SAFTLang.AST.Statements;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseStatements;

internal sealed partial class StatementParser
{
    private Statement ParseImportStatement()
    {
        Token importToken = _state.Consume(TokenType.Import);

        var path = new List<string>();

        Token first = _state.Consume(TokenType.Identifier);
        
        path.Add(first.Value);

        while (_state.Current.Type == TokenType.DoubleColon)
        {
            if (_state.Peek(1).Type == TokenType.LBrace)
            {
                break;
            }

            _state.Consume(TokenType.DoubleColon);

            Token part = _state.Consume(TokenType.Identifier);
            
            path.Add(part.Value);
        }

        List<string>? members = null;
        string? alias = null;

        if (_state.Current.Type == TokenType.DoubleColon &&
            _state.Peek(1).Type == TokenType.LBrace)
        {
            _state.Consume(TokenType.DoubleColon);
            _state.Consume(TokenType.LBrace);

            members = new List<string>();

            while (_state.Current.Type != TokenType.RBrace && !_state.IsAtEnd)
            {
                Token member = _state.Consume(TokenType.Identifier);
                members.Add(member.Value);

                if (_state.Current.Type != TokenType.Comma)
                {
                    break;
                }

                _state.Consume(TokenType.Comma);
            }

            _state.Consume(TokenType.RBrace);
        }

        if (_state.Current.Type == TokenType.As)
        {
            _state.Consume(TokenType.As);

            Token aliasToken = _state.Consume(TokenType.Identifier);
            
            alias = aliasToken.Value;
        }

        Token lastToken = _state.Current;
        _state.ConsumeStatementEnd();
        
        return new ImportStatement(path, members, alias, SourceSpan.Combine(importToken.Span, lastToken.Span));
    }
}