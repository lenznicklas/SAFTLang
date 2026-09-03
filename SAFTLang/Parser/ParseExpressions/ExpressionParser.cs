using SAFTLang.AST.Expressions;
using SAFTLang.Diagnostics;
using SAFTLang.Lexer.TokenAndKeywords;
using SAFTLang.Lexer.Text;

namespace SAFTLang.Parser.ParseExpressions;

internal sealed partial class ExpressionParser
{
    private readonly ParserState _state;
    private readonly DiagnosticBag _diagnostics;

    public ExpressionParser(ParserState state, DiagnosticBag diagnostics)
    {
        _state = state;
        _diagnostics = diagnostics;
    }

    public Expr ParseExpression()
    {
        return ParseOr();
    }
    
    private Expr FinishCall(Expr callee)
    {
        _state.Consume(TokenType.LParen);
        
        var arguments = new List<Expr>();

        if (_state.Current.Type != TokenType.RParen)
        {
            do
            {
                arguments.Add(ParseExpression());

                if (_state.Current.Type != TokenType.Comma)
                {
                    break;
                }

                _state.Consume(TokenType.Comma);
            } while (_state.Current.Type != TokenType.RParen);
        }
        
        Token closingParen = _state.Consume(TokenType.RParen);

        SourceSpan span = SourceSpan.Combine(callee.Span, closingParen.Span);

        return new CallExpr(callee, arguments, span);
    }

    private Expr FinishIndex(Expr target)
    {
        _state.Consume(TokenType.LBracket);

        Expr index = ParseExpression();

        Token closingBracket = _state.Consume(TokenType.RBracket);

        SourceSpan span = SourceSpan.Combine(target.Span, closingBracket.Span);

        return new IndexExpr(target, index, span);
    }


}