using SAFTLang.AST;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseTypes;

internal sealed partial class TypeParser
{
    public LangType? ParseOptionalType()
    {
        if (_state.Current.Type != TokenType.Colon)
        {
            return null;
        }

        _state.Consume(TokenType.Colon);
        
        return ParseType();
    }

}