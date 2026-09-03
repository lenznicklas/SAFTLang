using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseExpressions;

internal sealed partial class ExpressionParser
{
    private Expr ParseUnary()
    {
        if (_state.Current.Type == TokenType.Not ||
            _state.Current.Type == TokenType.Minus)
        {
            Token operatorToken = _state.Current;
            _state.Advance();

            Expr operand = ParseUnary();

            return new UnaryExpr(
                operatorToken.Type, operand, SourceSpan.Combine(operatorToken.Span, operand.Span)
            );
        }
        
        return ParsePostfix();
    }
}