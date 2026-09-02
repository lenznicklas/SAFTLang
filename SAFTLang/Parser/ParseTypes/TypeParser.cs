using SAFTLang.Diagnostics;

namespace SAFTLang.Parser.ParseTypes;

internal sealed partial class TypeParser
{
    private readonly ParserState _state;
    private readonly DiagnosticBag _diagnostics;

    public TypeParser(ParserState state, DiagnosticBag diagnostics)
    {
        _state = state;
        _diagnostics = diagnostics;
    }
}