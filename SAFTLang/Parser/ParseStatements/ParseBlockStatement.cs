using SAFTLang.AST;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseStatements;

internal sealed partial class StatementParser
{
    private BlockStatement ParseBlockStatement()
    {
        Token lBToken = _state.Consume(TokenType.LBrace);

        _state.SkipNewLines();

        var statements = new List<Statement>();

        while (_state.Current.Type != TokenType.RBrace && !_state.IsAtEnd)
        {
            Statement? statement = ParseStatement();

            if (statement is not null)
            {
                statements.Add(statement);
            }
            
            _state.SkipNewLines();
        }

        Token rBToken = _state.Consume(TokenType.RBrace);
        
        SourceSpan span = SourceSpan.Combine(lBToken.Span, rBToken.Span);
            
        BlockStatement block = new BlockStatement(statements, span);
        return block;
    }

}