using SAFTLang.AST;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseExpressions;

internal sealed partial class ExpressionParser
{
    private Expr ParseComparison()
    {
        Expr left = ParseAddition();

        while (_state.Current.Type == TokenType.Less ||
               _state.Current.Type == TokenType.LessEqual ||
               _state.Current.Type == TokenType.Greater ||
               _state.Current.Type == TokenType.GreaterEqual)
        {
            TokenType op = _state.Current.Type;
            _state.Advance();
            Expr right = ParseAddition();
            left = new BinaryExpr(left, op, right, SourceSpan.Combine(left.Span, right.Span));
        }
        
        return left;
    }

}