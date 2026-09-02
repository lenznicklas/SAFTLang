using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseExpressions;

internal sealed partial class ExpressionParser
{
    private Expr ParseAddition()
    {
        Expr left = ParseMultiplication();
        
        while (_state.Current.Type == TokenType.Plus || _state.Current.Type == TokenType.Minus)
        {
            TokenType op = _state.Current.Type;
            _state.Advance();

            Expr right = ParseMultiplication();
            left = new BinaryExpr(left, op, right, SourceSpan.Combine(left.Span, right.Span));
        }
        return left;
    }

}