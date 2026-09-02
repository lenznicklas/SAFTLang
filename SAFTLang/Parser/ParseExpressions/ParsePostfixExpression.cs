using SAFTLang.AST.Expressions;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseExpressions;

internal sealed partial class ExpressionParser
{
    private Expr ParsePostfix()
    {
        Expr expr = ParsePrimary();

        while (true)
        {
            if (_state.Current.Type == TokenType.LParen)
            {
                expr = FinishCall(expr);
                continue;
            }

            if (_state.Current.Type == TokenType.LBracket)
            {
                expr = FinishIndex(expr);
                continue;
            }
            
            break;
        }

        return expr;
    }

}