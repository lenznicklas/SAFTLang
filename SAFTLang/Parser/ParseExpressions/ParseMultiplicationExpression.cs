using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseExpressions;

internal sealed partial class ExpressionParser
{
    private Expr ParseMultiplication()
    {
        Expr left = ParsePostfix();

        while (_state.Current.Type == TokenType.Star || _state.Current.Type == TokenType.Slash)
        {
            TokenType op = _state.Current.Type;
            _state.Advance();
            Expr right = ParsePostfix();
            
            left = new BinaryExpr(left, op, right, SourceSpan.Combine(left.Span, right.Span));
        }
        return left;
    }

}