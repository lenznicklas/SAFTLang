using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Lexer.Readers;

internal sealed partial class TokenReader
{
    public Token ReadIdentifier()
    {
        int start = _state.Position;
        int line = _state.Line;
        int column = _state.Column;

        while (!_state.IsAtEnd && (char.IsLetterOrDigit(_state.Current) || _state.Current == '_'))
        {
            _state.Advance();
        } 

        string value = _state.Source[start.._state.Position];

        TokenType type = KeywordsDict.Keywords.TryGetValue(
            value,
            out TokenType keywordType)
            ? keywordType
            : TokenType.Identifier;
            
        return _state.CreateToken(
            type,
            value,
            start,
            _state.Position - start,
            line,
            column
        );
    }
}