using SAFTLang.Lexer.TokenAndKeywords;
using SAFTLang.Lexer.Text;
using SAFTLang.AST.Expressions;
using SAFTLang.AST.Statements;

namespace SAFTLang.Parser.ParseStatements;

internal sealed partial class StatementParser
{
    private Statement ParseIfStatement()
    {
        Token ifToken = _state.Consume(TokenType.If);

        Expr condition = _expressionParser.ParseExpression();

        BlockStatement thenBody = ParseBlockStatement();

        BlockStatement? elseBody = null;
        
        _state.SkipNewLines();

        if (_state.Current.Type == TokenType.Else)
        {
            _state.Consume(TokenType.Else);

            if (_state.Current.Type == TokenType.If)
            {
                Statement ifElse = ParseIfStatement();
                
                elseBody = new BlockStatement(new List<Statement>{ifElse}, ifElse.Span);
            }
            else
            {
                elseBody = ParseBlockStatement();
            }
        }

        SourceSpan lastSpan = elseBody?.Span ?? thenBody.Span;
        
        SourceSpan span = SourceSpan.Combine(ifToken.Span, lastSpan);

        return new IfStatement(condition, thenBody, elseBody, span);
    }

}