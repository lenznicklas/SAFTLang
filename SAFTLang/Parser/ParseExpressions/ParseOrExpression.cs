using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseExpressions;

internal sealed partial class ExpressionParser
{
    private Expr ParseOr()
    {
        Expr left = ParseAnd();

        while (_state.Current.Type == TokenType.Or)
        {
            TokenType op = _state.Current.Type;
            _state.Advance();

            Expr right = ParseAnd();
            
            SourceSpan span = SourceSpan.Combine(left.Span, right.Span);
            
            left = new BinaryExpr(left, op, right, span);
        }
        
        return left;
    }
}