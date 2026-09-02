using SAFTLang.Lexer.TokenAndKeywords;
using SAFTLang.Lexer.Text;
using SAFTLang.AST;

namespace SAFTLang.Parser.ParseStatements;

internal sealed partial class StatementParser
{
    private Statement ParseIdentifierStartedStatement()
    {
        Expr left = _expressionParser.ParseExpression();

        if (_state.Current.Type == TokenType.Equal)
        {
            _state.Consume(TokenType.Equal);

            Expr value = _expressionParser.ParseExpression();
            
            _state.ConsumeStatementEnd();

            SourceSpan span = SourceSpan.Combine(left.Span, value.Span);

            return new AssignmentStatement(left, value, span);
        }
        
        _state.ConsumeStatementEnd();

        return new ExpressionStatement(left, left.Span);
    }

}