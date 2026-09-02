using SAFTLang.Lexer.TokenAndKeywords;
using SAFTLang.AST;

namespace SAFTLang.Parser.ParseStatements;

internal sealed partial class StatementParser
{
    private Statement ParseReturnStatement()
    {
        Token returnToken = _state.Consume(TokenType.Return);

        Expr? value = null;
        
        bool hasValue =
            _state.Current.Type != TokenType.RBrace &&
            _state.Current.Type != TokenType.Newline &&
            _state.Current.Type != TokenType.EOF &&
            _state.Current.Type != TokenType.Semicolon;

        if (hasValue)
        {
            value = _expressionParser.ParseExpression();
        }
        
        _state.ConsumeStatementEnd();
        
        return new ReturnStatement(value,  returnToken.Span);
    }

}