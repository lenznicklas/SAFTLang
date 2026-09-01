using SAFTLang.Diagnostics;

namespace SAFTLang.Lexer.Readers;

internal sealed partial class TokenReader
{
    private readonly LexerState _state;
    private readonly DiagnosticBag _diagnostics;
    

    public TokenReader(LexerState state, DiagnosticBag diagnostics)
    {
        _state = state;
        _diagnostics = diagnostics;
    }
}