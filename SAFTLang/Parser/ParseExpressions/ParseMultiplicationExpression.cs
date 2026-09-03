using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseExpressions;

internal sealed partial class ExpressionParser
{
    private Expr ParseMultiplication()
    {
        Expr left = ParseUnary();

        while (_state.Current.Type == TokenType.Star || 
               _state.Current.Type == TokenType.Slash ||
               _state.Current.Type == TokenType.Modulo)
        {
            TokenType op = _state.Current.Type;
            _state.Advance();
            Expr right = ParseUnary();
            
            left = new BinaryExpr(left, op, right, SourceSpan.Combine(left.Span, right.Span));
        }
        return left;
    }

}