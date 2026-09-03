using SAFTLang.AST.Expressions;
using SAFTLang.AST.Statements;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseStatements;

internal sealed partial class StatementParser
{
    private Statement ParseForStatement()
    {
        Token forToken = _state.Consume(TokenType.For);
        
        Expr? condition = null;
        BlockStatement block;
        SourceSpan span;

        if (_state.Current.Type != TokenType.LBrace)
        {
            condition = _expressionParser.ParseExpression();
            block = ParseBlockStatement();
            span = SourceSpan.Combine(forToken.Span, block.Span);
            
            return new ForStatement(condition, block, span);
        }

        block = ParseBlockStatement();
        span = SourceSpan.Combine(forToken.Span, block.Span);
        
        return new ForStatement(null, block, span);
    }
}