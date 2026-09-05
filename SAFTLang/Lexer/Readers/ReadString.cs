using System.Text;
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
        
        var value = new StringBuilder();

        bool hasError = false;
        
        while (!_state.IsAtEnd && _state.Current != '"' && _state.Current != '\n')
        {
            if (_state.Current == '\\')
            {
                int escapeStart = _state.Position;
                int escapeLine = _state.Line;
                int escapeColumn = _state.Column;
                
                _state.Advance();

                if (_state.IsAtEnd || _state.Current == '\n')
                {
                    break;
                }

                char escaped;

                switch (_state.Current)
                {
                    case 'n':
                        escaped = '\n';
                        break;
                    case 't':
                        escaped = '\t';
                        break;
                    case 'r':
                        escaped = '\r';
                        break;
                    case '"':
                        escaped = '"';
                        break;
                    case '\\':
                        escaped = '\\';
                        break;
                    default:
                        _diagnostics.ReportError(
                            new SourceSpan(escapeStart, 2, escapeLine, escapeColumn),
                            $"Unknown escape sequence '\\{_state.Current}'"
                        );
                        hasError = true;
                        
                        _state.Advance();
                        continue;
                }
                
                value.Append(escaped);
                
                _state.Advance();
                continue;
            }

            value.Append(_state.Current);
            _state.Advance();
        }

        if (_state.IsAtEnd || _state.Current == '\n')
        {
            _diagnostics.ReportError(
                new SourceSpan(start, _state.Position - start, line, column),
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
        
        _state.Advance();

        if (hasError)
        {
            return _state.CreateToken(
                TokenType.BadToken,
                "",
                start,
                _state.Position - start,
                line,
                column
            );
        }
        
        return _state.CreateToken(
            TokenType.String,
            value.ToString(),
            start,
            _state.Position - start,
            line,
            column
        );
    }
}