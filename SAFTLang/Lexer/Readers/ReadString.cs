using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Lexer.Readers;

internal sealed partial class TokenReader
{
    public Token ReadString()
    {
        int start = _state.Position;
        int line = _state.Line;
        int column = _state.Column;
        
        _state.Advance();

        int valueStart = _state.Position;
        
        while (!_state.IsAtEnd && _state.Current != '"' && _state.Current != '\n')
        {
            _state.Advance();
        }

        if (_state.IsAtEnd || _state.Current == '\n')
        {
            _diagnostics.ReportError(
                new SourceSpan(start, _state.Position - start, line, column),
                "Unterminated string"
            );
        }

        if (_state.IsAtEnd)
        {
            _diagnostics.ReportError(
                new SourceSpan(
                    start,
                    _state.Position - start,
                    line,
                    column),
                "Unterminated string"
            );

            return _state.CreateToken(
                TokenType.BadToken,
                "",
                start,
                _state.Position - start,
                line,
                column
            );
        }
        
        string value = _state.Source[valueStart.._state.Position];
        
        _state.Advance();

        return _state.CreateToken(
            TokenType.String,
            value,
            start,
            _state.Position - start,
            line,
            column
        );
    }
}