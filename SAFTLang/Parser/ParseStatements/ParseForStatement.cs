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

        if (_state.Current.Type != TokenType.LBrace &&
            _state.Peek(1).Type != TokenType.In)
        {
            condition = _expressionParser.ParseExpression();
            block = ParseBlockStatement();
            span = SourceSpan.Combine(forToken.Span, block.Span);
            
            return new ForStatement(condition, block, span);
        }

        if (_state.Current.Type == TokenType.Identifier &&
            _state.Peek(1).Type == TokenType.In)
        {
            Token variable = _state.Consume(TokenType.Identifier);
            _state.Consume(TokenType.In);
            Expr iter = _expressionParser.ParseExpression();
            block = ParseBlockStatement();
            span = SourceSpan.Combine(forToken.Span, block.Span);
            
            return new ForEachStatement(variable.Value,  iter, block, span);
        }

        block = ParseBlockStatement();
        span = SourceSpan.Combine(forToken.Span, block.Span);
        
        return new ForStatement(null, block, span);
    }
}