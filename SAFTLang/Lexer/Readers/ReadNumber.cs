using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Lexer.Readers;

internal sealed partial class TokenReader
{
    public Token ReadNumber()
    {
        int start = _state.Position;
        int line = _state.Line;
        int column = _state.Column;

        while (!_state.IsAtEnd && (char.IsDigit(_state.Current)))
        {
            _state.Advance();
        }

        string value = _state.Source[start.._state.Position];
        
        return _state.CreateToken(
            TokenType.Number,
            value,
            start,
            _state.Position - start,
            line,
            column
        );
    }
}