using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseExpressions;

internal sealed partial class ExpressionParser
{
    private Expr ParseArrayExpression()
    {
        Token openingBracket = _state.Consume(TokenType.LBracket);
        var elements = new List<Expr>();

        while (_state.Current.Type != TokenType.RBracket && !_state.IsAtEnd)
        {
            elements.Add(ParseExpression());

            if (_state.Current.Type != TokenType.Comma)
            {
                break;
            }

            _state.Consume(TokenType.Comma);
        }
        
        Token closingBracket = _state.Consume(TokenType.RBracket);

        SourceSpan span = SourceSpan.Combine(openingBracket.Span, closingBracket.Span);
        
        return new ArrayExpr(elements, span);
    }

}