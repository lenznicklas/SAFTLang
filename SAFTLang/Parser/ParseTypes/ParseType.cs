using SAFTLang.AST.Types;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseTypes;

internal sealed partial class TypeParser
{
    public LangType ParseType()
    {
        Token token = _state.Current;

        LangType type = token.Type switch
        {
            TokenType.IntType => LangType.Int,
            TokenType.BoolType => LangType.Bool,
            TokenType.StringType => LangType.String,
            TokenType.CharType => LangType.Char,
            TokenType.VoidType => LangType.Void,
            _ => LangType.Error
        };

        if (type == LangType.Error)
        {
            _diagnostics.ReportError(token.Span,
                $"Expected Type, got {token.Type}"
            );
            
            _state.Advance();

            return LangType.Error;
        }
        
        _state.Advance();

        while (_state.Current.Type == TokenType.LBracket &&
               _state.Peek(1).Type == TokenType.RBracket)
        {
            _state.Consume(TokenType.LBracket);
            _state.Consume(TokenType.RBracket);

            if (type == LangType.Void)
            {
                _diagnostics.ReportError(
                    token.Span,
                    "Array element type cannot be void"
                );

                type = LangType.Error;
                continue;
            }

            if (type != LangType.Error)
            {
                type = LangType.ArrayOf(type);
            }
        }

        return type;
    }

}