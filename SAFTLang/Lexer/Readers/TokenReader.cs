using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Lexer.Readers;

internal sealed partial class TokenReader
{
    private readonly LexerState _state;

    public TokenReader(LexerState state)
    {
        _state = state;
    }
}