using SAFTLang.Lexer.TokenAndKeywords;
using SAFTLang.Lexer.Text;
using SAFTLang.AST;

namespace SAFTLang.Parser.ParseStatements;

internal sealed partial class StatementParser
{
    private Statement ParseFunctionStatement()
    {
        Token funcToken = _state.Consume(TokenType.Func);
        
        Token name = _state.Consume(TokenType.Identifier);

        _state.Consume(TokenType.LParen);
        
        var parameters = new List<Parameter>();

        if (_state.Current.Type != TokenType.RParen)
        {
            while (true)
            {
                Token parameterName = _state.Consume(TokenType.Identifier);

                _state.Consume(TokenType.Colon);

                LangType parameterType = _typeParser.ParseType();

                parameters.Add(new Parameter(parameterName.Value, parameterType, parameterName.Span));

                if (_state.Current.Type != TokenType.Comma)
                {
                    break;
                }

                _state.Consume(TokenType.Comma);
            }
        }

        _state.Consume(TokenType.RParen);
            
        LangType returnType = _typeParser.ParseType();
            
        BlockStatement body = ParseBlockStatement();

        SourceSpan span = SourceSpan.Combine(funcToken.Span, body.Span);

        return new FunctionStatement(
            name.Value,
            parameters,
            returnType,
            body,
            span
        );

    }

}