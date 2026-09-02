using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.TokenAndKeywords;
using SAFTLang.Lexer.Text;

namespace SAFTLang.Parser.ParseExpressions;

internal sealed partial class ExpressionParser
{
    private Expr ParseEquality()
    {
        Expr left = ParseComparison();

        while (_state.Current.Type == TokenType.EqualEqual ||
               _state.Current.Type == TokenType.NotEqual)
        {
            TokenType op = _state.Current.Type;
            _state.Advance();

            Expr right = ParseComparison();
            
            left = new BinaryExpr(left, op, right, SourceSpan.Combine(left.Span, right.Span));
        }
        
        return left;
    }

}