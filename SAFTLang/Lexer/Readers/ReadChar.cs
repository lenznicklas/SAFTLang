using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Lexer.Readers;

internal sealed partial class TokenReader
{
    public Token ReadChar()
    {
        int start = _state.Position;
        int line = _state.Line;
        int column = _state.Column;
        
        _state.Advance();

        int valuePosition = _state.Position;

        while (_state.Current != '\'' &&
               _state.Current != '\n' &&
               !_state.IsAtEnd)
        {
            _state.Advance();
        }

        if (_state.IsAtEnd || _state.Current == '\n')
        {
            _diagnostics.ReportError(
                new SourceSpan(start, _state.Position - start, line, column),
                "Unterminated char literal"
            );
            
            return _state.CreateToken(TokenType.BadToken, "", start, _state.Position-start,  line, column);
        }

        string value = _state.Source[valuePosition.._state.Position];
        
        _state.Advance();

        if (value.Length != 1)
        {
            _diagnostics.ReportError(
                new SourceSpan(start, _state.Position - start, line, column),
                "Char literal must contain exactly one character"
            );
            
            return _state.CreateToken(TokenType.BadToken, "", start, _state.Position-start,  line, column);
        }

        if (value[0] > 127 || char.IsControl(value[0]))
        {
            _diagnostics.ReportError(
                new SourceSpan(start, _state.Position - start, line, column),
                "Char literal must be an ASCII-character"
            );
            
            return _state.CreateToken(TokenType.BadToken, "", start, _state.Position-start,  line, column);
        }
        
        return _state.CreateToken(TokenType.Char, value, start, _state.Position-start, line, column);
    }
}